#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiantesLote;

internal sealed class CreateEstudiantesLoteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateEstudiantesLoteCommand, List<EstudianteDto>>
{
    public async Task<List<EstudianteDto>> Handle(CreateEstudiantesLoteCommand request, CancellationToken ct)
    {
        // Validación previa: detectar duplicados dentro del propio lote
        var cedulas = request.Estudiantes.Select(e => e.Cedula).ToList();
        var correos = request.Estudiantes.Select(e => e.Correo.Trim().ToLowerInvariant()).ToList();

        if (cedulas.Count != cedulas.Distinct().Count())
            throw new BadRequestException("El lote contiene cédulas duplicadas entre los estudiantes a registrar.");

        if (correos.Count != correos.Distinct().Count())
            throw new BadRequestException("El lote contiene correos duplicados entre los estudiantes a registrar.");

        // Verificar duplicados contra DB en una sola consulta
        var cedulasExistentes = await db.Estudiantes
            .Where(e => cedulas.Contains(e.Cedula))
            .Select(e => e.Cedula)
            .ToListAsync(ct);

        if (cedulasExistentes.Count != 0)
            throw new BadRequestException(
                $"Las siguientes cédulas ya están registradas: {string.Join(", ", cedulasExistentes)}.");

        var correosExistentes = await db.Estudiantes
            .Where(e => correos.Contains(e.Correo))
            .Select(e => e.Correo)
            .ToListAsync(ct);

        if (correosExistentes.Count != 0)
            throw new BadRequestException(
                $"Los siguientes correos ya están registrados: {string.Join(", ", correosExistentes)}.");

        var creados = new List<Estudiante>();
        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            foreach (var dto in request.Estudiantes)
            {
                var est = Estudiante.Create(dto.NombreCompleto, dto.Cedula, dto.Correo);
                _ = await db.Estudiantes.AddAsync(est, ct);
                creados.Add(est);
            }

            _ = await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return creados.Adapt<List<EstudianteDto>>();
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
