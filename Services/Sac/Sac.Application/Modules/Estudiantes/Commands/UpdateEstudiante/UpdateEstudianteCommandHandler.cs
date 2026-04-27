#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.UpdateEstudiante;

internal sealed class UpdateEstudianteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateEstudianteCommand, EstudianteDto>
{
    public async Task<EstudianteDto> Handle(UpdateEstudianteCommand request, CancellationToken ct)
    {
        var estudiante = await db.Estudiantes.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
                         ?? throw new NotFoundException("Estudiante", request.Id);

        var correoNorm = request.Estudiante.Correo.Trim().ToLowerInvariant();

        if (await db.Estudiantes.AnyAsync(e => e.Correo == correoNorm && e.Id != request.Id, ct))
            throw new BadRequestException("El correo electrónico ya está registrado por otro estudiante.");

        estudiante.Actualizar(request.Estudiante.NombreCompleto, request.Estudiante.Correo);
        _ = await db.SaveChangesAsync(ct);

        return estudiante.Adapt<EstudianteDto>();
    }
}
