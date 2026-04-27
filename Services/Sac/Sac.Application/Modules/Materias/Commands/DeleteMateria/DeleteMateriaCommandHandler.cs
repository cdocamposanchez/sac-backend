#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.Materias.Commands.DeleteMateria;

internal sealed class DeleteMateriaCommandHandler(IApplicationDbContext db)
    : ICommandHandler<DeleteMateriaCommand, bool>
{
    public async Task<bool> Handle(DeleteMateriaCommand request, CancellationToken ct)
    {
        var materia = await db.Materias.FirstOrDefaultAsync(m => m.Id == request.Id, ct)
                      ?? throw new NotFoundException("Materia", request.Id);

        if (await db.Cursos.AnyAsync(c => c.MateriaId == request.Id, ct))
            throw new BadRequestException("No se puede inactivar: la materia está siendo usada por uno o más cursos.");

        materia.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
