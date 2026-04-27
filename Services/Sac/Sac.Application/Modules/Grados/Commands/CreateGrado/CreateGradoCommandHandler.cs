#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Grados.Commands.CreateGrado;

internal sealed class CreateGradoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateGradoCommand, GradoDto>
{
    public async Task<GradoDto> Handle(CreateGradoCommand request, CancellationToken ct)
    {
        var nombreUpper = request.Grado.Nombre.Trim().ToUpperInvariant();
        if (await db.Grados.AnyAsync(g => g.Nombre == nombreUpper, ct))
            throw new BadRequestException($"El grado '{nombreUpper}' ya existe.");

        var grado = Grado.Create(request.Grado.Nombre, request.Grado.Descripcion);
        _ = await db.Grados.AddAsync(grado, ct);
        _ = await db.SaveChangesAsync(ct);

        return grado.Adapt<GradoDto>();
    }
}
