#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.UpdateDocente;

internal sealed class UpdateDocenteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateDocenteCommand, DocenteDto>
{
    public async Task<DocenteDto> Handle(UpdateDocenteCommand request, CancellationToken ct)
    {
        var docente = await db.Docentes.FirstOrDefaultAsync(d => d.Id == request.Id, ct)
                      ?? throw new NotFoundException("Docente", request.Id);

        var nuevoCorreo = request.Docente.Correo.Trim().ToLowerInvariant();

        if (await db.Docentes.AnyAsync(d => d.Correo == nuevoCorreo && d.Id != request.Id, ct))
            throw new BadRequestException("El correo electrónico ya está registrado por otro docente.");

        if (await db.Directores.AnyAsync(d => d.Correo == nuevoCorreo, ct))
            throw new BadRequestException("El correo electrónico ya está registrado en el sistema.");

        docente.Actualizar(request.Docente.NombreCompleto, request.Docente.Correo);

        _ = await db.SaveChangesAsync(ct);
        return docente.Adapt<DocenteDto>();
    }
}
