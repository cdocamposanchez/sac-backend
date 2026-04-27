#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.DeleteEstudiante;

internal sealed class DeleteEstudianteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<DeleteEstudianteCommand, bool>
{
    public async Task<bool> Handle(DeleteEstudianteCommand request, CancellationToken ct)
    {
        var estudiante = await db.Estudiantes.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
                         ?? throw new NotFoundException("Estudiante", request.Id);

        estudiante.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
