#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.CreateDocente;

internal sealed class CreateDocenteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateDocenteCommand, DocenteDto>
{
    public async Task<DocenteDto> Handle(CreateDocenteCommand request, CancellationToken ct)
    {
        var dto = request.Docente;
        var correoNormalizado = dto.Correo.Trim().ToLowerInvariant();

        if (await db.Docentes.AnyAsync(d => d.Cedula == dto.Cedula, ct))
            throw new BadRequestException("La cédula ya se encuentra registrada como docente.");

        if (await db.Docentes.AnyAsync(d => d.Correo == correoNormalizado, ct))
            throw new BadRequestException("El correo electrónico ya está registrado como docente.");

        // Verificar que no colisione con un Director (RF-02 establece unicidad por cédula y correo)
        if (await db.Directores.AnyAsync(d => d.Cedula == dto.Cedula, ct))
            throw new BadRequestException("La cédula ya está registrada en el sistema.");

        if (await db.Directores.AnyAsync(d => d.Correo == correoNormalizado, ct))
            throw new BadRequestException("El correo electrónico ya está registrado en el sistema.");

        var docente = Docente.Create(dto.NombreCompleto, dto.Cedula, dto.Correo, dto.Password);

        _ = await db.Docentes.AddAsync(docente, ct);
        _ = await db.SaveChangesAsync(ct);

        return docente.Adapt<DocenteDto>();
    }
}
