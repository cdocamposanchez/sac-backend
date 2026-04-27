#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.Grados.Commands.DeleteGrado;

internal sealed class DeleteGradoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<DeleteGradoCommand, bool>
{
    public async Task<bool> Handle(DeleteGradoCommand request, CancellationToken ct)
    {
        var grado = await db.Grados.FirstOrDefaultAsync(g => g.Id == request.Id, ct)
                    ?? throw new NotFoundException("Grado", request.Id);

        if (await db.Cursos.AnyAsync(c => c.GradoId == request.Id, ct))
            throw new BadRequestException("No se puede inactivar: el grado está siendo usado por uno o más cursos.");

        grado.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
