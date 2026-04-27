#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Materias.Commands.CreateMateria;

internal sealed class CreateMateriaCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateMateriaCommand, MateriaDto>
{
    public async Task<MateriaDto> Handle(CreateMateriaCommand request, CancellationToken ct)
    {
        var nombre = request.Materia.Nombre.Trim();
        if (await db.Materias.AnyAsync(m => m.Nombre == nombre, ct))
            throw new BadRequestException($"La materia '{nombre}' ya existe.");

        var materia = Materia.Create(request.Materia.Nombre, request.Materia.Descripcion);
        _ = await db.Materias.AddAsync(materia, ct);
        _ = await db.SaveChangesAsync(ct);
        return materia.Adapt<MateriaDto>();
    }
}
