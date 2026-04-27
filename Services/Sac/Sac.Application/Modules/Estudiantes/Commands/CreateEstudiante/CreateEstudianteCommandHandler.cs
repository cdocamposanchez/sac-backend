#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiante;

internal sealed class CreateEstudianteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateEstudianteCommand, EstudianteDto>
{
    public async Task<EstudianteDto> Handle(CreateEstudianteCommand request, CancellationToken ct)
    {
        var dto = request.Estudiante;
        var correoNorm = dto.Correo.Trim().ToLowerInvariant();

        if (await db.Estudiantes.AnyAsync(e => e.Cedula == dto.Cedula, ct))
            throw new BadRequestException("La cédula ya se encuentra registrada como estudiante.");

        if (await db.Estudiantes.AnyAsync(e => e.Correo == correoNorm, ct))
            throw new BadRequestException("El correo electrónico ya está registrado como estudiante.");

        var estudiante = Estudiante.Create(dto.NombreCompleto, dto.Cedula, dto.Correo);

        _ = await db.Estudiantes.AddAsync(estudiante, ct);
        _ = await db.SaveChangesAsync(ct);

        return estudiante.Adapt<EstudianteDto>();
    }
}
