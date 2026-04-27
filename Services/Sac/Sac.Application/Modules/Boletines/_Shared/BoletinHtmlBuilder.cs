#region

using System.Globalization;
using System.Text;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Boletines._Shared;

/// <summary>
/// Helper interno del módulo Boletines. Construye el cuerpo HTML del correo
/// con el detalle de notas y promedios de un estudiante.
/// </summary>
internal static class BoletinHtmlBuilder
{
    public static string Construir(ReporteEstudianteDto reporte)
    {
        var sb = new StringBuilder();

        sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'><style>")
          .Append("body{font-family:Arial,sans-serif;color:#222;max-width:720px;margin:0 auto;padding:20px;}")
          .Append("h1{color:#1a3d7c;font-size:22px;margin-bottom:4px;}")
          .Append("h2{color:#2a5599;border-bottom:2px solid #2a5599;padding-bottom:4px;margin-top:24px;}")
          .Append(".header{background:#eaf0fb;padding:14px;border-radius:6px;margin-bottom:18px;}")
          .Append("table{border-collapse:collapse;width:100%;margin:8px 0;}")
          .Append("th,td{border:1px solid #ddd;padding:8px;text-align:left;font-size:13px;}")
          .Append("th{background:#f4f6fa;font-weight:600;}")
          .Append(".prom{padding:8px;background:#f9f9fb;border-left:4px solid #2a5599;margin:8px 0;}")
          .Append(".final{font-size:18px;font-weight:bold;color:#fff;background:#1a3d7c;")
          .Append("padding:14px;border-radius:6px;margin-top:24px;text-align:center;}")
          .Append(".footer{margin-top:30px;font-size:11px;color:#888;text-align:center;}")
          .Append("</style></head><body>");

        sb.Append("<div class='header'>")
          .Append("<h1>Sistema Académico Cayzedo (SAC)</h1>")
          .Append("<p style='margin:0;font-size:14px;'>Boletín académico final</p>")
          .Append("</div>");

        sb.Append($"<p><strong>Estudiante:</strong> {Esc(reporte.EstudianteNombre)} (CC {Esc(reporte.Cedula)})<br/>")
          .Append($"<strong>Curso:</strong> {Esc(reporte.CursoNombre)}</p>");

        foreach (var p in reporte.Periodos)
        {
            sb.Append($"<h2>Corte {p.NumeroCorte} <small style='font-weight:normal;color:#666;'>({Esc(p.Estado)})</small></h2>");

            if (p.Calificaciones.Count == 0)
            {
                sb.Append("<p style='color:#999;'>Sin calificaciones registradas en este corte.</p>");
            }
            else
            {
                sb.Append("<table><thead><tr><th>Actividad</th><th style='width:90px;'>Nota</th><th style='width:90px;'>Porcentaje</th></tr></thead><tbody>");
                foreach (var c in p.Calificaciones)
                    sb.Append($"<tr><td>{Esc(c.ActividadNombre)}</td>")
                      .Append($"<td>{c.Nota.ToString("0.00", CultureInfo.InvariantCulture)}</td>")
                      .Append($"<td>{c.Porcentaje.ToString("0.##", CultureInfo.InvariantCulture)}%</td></tr>");
                sb.Append("</tbody></table>");
            }

            var promStr = p.PromedioPeriodo?.ToString("0.00", CultureInfo.InvariantCulture) ?? "—";
            sb.Append($"<div class='prom'><strong>Promedio del corte {p.NumeroCorte}:</strong> {promStr}</div>");
        }

        var finalStr = reporte.PromedioFinal?.ToString("0.00", CultureInfo.InvariantCulture) ?? "—";
        sb.Append($"<div class='final'>Promedio final del curso: {finalStr}</div>");

        sb.Append("<div class='footer'>")
          .Append("Este es un boletín generado automáticamente por el Sistema Académico Cayzedo (SAC) ")
          .Append("de la Institución Educativa Joaquín de Cayzedo y Cuero. ")
          .Append("Por favor, no responda este correo.")
          .Append("</div>");

        sb.Append("</body></html>");
        return sb.ToString();
    }

    private static string Esc(string? s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
