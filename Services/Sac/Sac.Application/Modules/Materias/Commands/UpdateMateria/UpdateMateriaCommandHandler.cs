#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Commands.UpdateMateria;

internal sealed class UpdateMateriaCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateMateriaCommand, MateriaDto>
{
    public async Task<MateriaDto> Handle(UpdateMateriaCommand request, CancellationToken ct)
    {
        var materia = await db.Materias.FirstOrDefaultAsync(m => m.Id == request.Id, ct)
                      ?? throw new NotFoundException("Materia", request.Id);

        var nombre = request.Materia.Nombre.Trim();
        if (await db.Materias.AnyAsync(m => m.Nombre == nombre && m.Id != request.Id, ct))
            throw new BadRequestException("Ya existe otra materia con ese nombre.");

        materia.Actualizar(request.Materia.Nombre, request.Materia.Descripcion);
        _ = await db.SaveChangesAsync(ct);
        return materia.Adapt<MateriaDto>();
    }
}
