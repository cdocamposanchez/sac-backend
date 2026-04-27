#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Commands.UpdateGrado;

internal sealed class UpdateGradoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateGradoCommand, GradoDto>
{
    public async Task<GradoDto> Handle(UpdateGradoCommand request, CancellationToken ct)
    {
        var grado = await db.Grados.FirstOrDefaultAsync(g => g.Id == request.Id, ct)
                    ?? throw new NotFoundException("Grado", request.Id);

        var nombreUpper = request.Grado.Nombre.Trim().ToUpperInvariant();
        if (await db.Grados.AnyAsync(g => g.Nombre == nombreUpper && g.Id != request.Id, ct))
            throw new BadRequestException("Ya existe otro grado con ese nombre.");

        grado.Actualizar(request.Grado.Nombre, request.Grado.Descripcion);
        _ = await db.SaveChangesAsync(ct);
        return grado.Adapt<GradoDto>();
    }
}
