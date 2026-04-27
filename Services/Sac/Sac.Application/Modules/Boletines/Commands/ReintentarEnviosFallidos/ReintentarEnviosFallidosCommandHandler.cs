#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Boletines._Shared;
using Sac.Application.Modules.Reportes._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Enums;

#endregion

namespace Sac.Application.Modules.Boletines.Commands.ReintentarEnviosFallidos;

internal sealed class ReintentarEnviosFallidosCommandHandler(
    IApplicationDbContext db,
    IEmailService emailService) : ICommandHandler<ReintentarEnviosFallidosCommand, EnviarBoletinesResultadoDto>
{
    public async Task<EnviarBoletinesResultadoDto> Handle(
        ReintentarEnviosFallidosCommand request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CursoId, ct)
            ?? throw new NotFoundException("Curso", request.CursoId);

        var fallidos = await db.EnviosBoletin
            .Where(e => e.CursoId == request.CursoId && e.Estado == EstadoEnvioBoletin.Fallido)
            .ToListAsync(ct);

        int exitosos = 0, restantes = 0;
        var detalles = new List<EnvioBoletinDto>(fallidos.Count);

        foreach (var envio in fallidos)
        {
            var est = await db.Estudiantes.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == envio.EstudianteId, ct);
            if (est is null) continue;

            var reporte = await ReporteBuilder.ConstruirReporteEstudianteAsync(db, curso, est, ct);
            var html = BoletinHtmlBuilder.Construir(reporte);

            envio.EncolarReintento();
            var subject = $"[SAC] Boletín final - {curso.Nombre}";
            var resultado = await emailService.SendAsync(envio.CorreoDestino, est.NombreCompleto, subject, html, ct);

            if (resultado.Success) { envio.MarcarComoEnviado(); exitosos++; }
            else
            {
                envio.MarcarComoFallido(resultado.ErrorMessage ?? "Error desconocido al enviar correo.");
                restantes++;
            }

            _ = await db.SaveChangesAsync(ct);

            detalles.Add(new EnvioBoletinDto(
                envio.Id, envio.CursoId, envio.EstudianteId, envio.CorreoDestino,
                envio.Estado.ToString(), envio.FechaEnvio, envio.MotivoFallo, envio.Intentos));
        }

        return new EnviarBoletinesResultadoDto(curso.Id, fallidos.Count, exitosos, restantes, detalles);
    }
}
