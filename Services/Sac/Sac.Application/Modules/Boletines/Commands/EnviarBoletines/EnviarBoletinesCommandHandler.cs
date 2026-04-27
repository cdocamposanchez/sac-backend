#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Boletines._Shared;
using Sac.Application.Modules.Reportes._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Boletines.Commands.EnviarBoletines;

internal sealed class EnviarBoletinesCommandHandler(
    IApplicationDbContext db,
    IEmailService emailService) : ICommandHandler<EnviarBoletinesCommand, EnviarBoletinesResultadoDto>
{
    public async Task<EnviarBoletinesResultadoDto> Handle(EnviarBoletinesCommand request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CursoId, ct)
            ?? throw new NotFoundException("Curso", request.CursoId);

        // RF-11: solo enviar si el curso está cerrado
        if (!curso.EstaCerrado)
            throw new BadRequestException(
                "El envío de boletines solo está disponible cuando el curso está CERRADO.");

        var estudianteIds = await db.CursoEstudiantes.AsNoTracking()
            .Where(ce => ce.CursoId == request.CursoId && ce.Activo)
            .Select(ce => ce.EstudianteId)
            .ToListAsync(ct);

        var estudiantes = await db.Estudiantes.AsNoTracking()
            .Where(e => estudianteIds.Contains(e.Id))
            .ToListAsync(ct);

        int exitosos = 0, fallidos = 0;
        var detalles = new List<EnvioBoletinDto>(estudiantes.Count);

        foreach (var est in estudiantes)
        {
            // Construir reporte y HTML
            var reporte = await ReporteBuilder.ConstruirReporteEstudianteAsync(db, curso, est, ct);
            var htmlBody = BoletinHtmlBuilder.Construir(reporte);

            // Crear el envío en estado pendiente
            var envio = EnvioBoletin.Create(curso.Id, est.Id, est.Correo);
            _ = await db.EnviosBoletin.AddAsync(envio, ct);
            _ = await db.SaveChangesAsync(ct);

            try
            {
                var subject = $"[SAC] Boletín final - {curso.Nombre}";
                var resultado = await emailService.SendAsync(est.Correo, est.NombreCompleto, subject, htmlBody, ct);

                if (resultado.Success)
                {
                    envio.MarcarComoEnviado();
                    exitosos++;
                }
                else
                {
                    envio.MarcarComoFallido(resultado.ErrorMessage ?? "Error desconocido al enviar correo.");
                    fallidos++;
                }
            }
            catch (Exception ex)
            {
                envio.MarcarComoFallido(ex.Message);
                fallidos++;
            }

            _ = await db.SaveChangesAsync(ct);

            detalles.Add(new EnvioBoletinDto(
                envio.Id, envio.CursoId, envio.EstudianteId, envio.CorreoDestino,
                envio.Estado.ToString(), envio.FechaEnvio, envio.MotivoFallo, envio.Intentos));
        }

        return new EnviarBoletinesResultadoDto(curso.Id, estudiantes.Count, exitosos, fallidos, detalles);
    }
}
