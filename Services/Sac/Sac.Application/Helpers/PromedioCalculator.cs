#region

using Sac.Domain.Models;

#endregion

namespace Sac.Application.Helpers;

/// <summary>
/// Servicio de dominio para cálculo automático de promedios (RF-09).
///
/// <para>
/// El porcentaje vive en la <see cref="Actividad"/>: cada calificación aporta
/// <c>nota × actividad.Porcentaje / 100</c>. Por eso los métodos reciben además
/// un diccionario <c>actividades[id] = Actividad</c> para resolver el peso de cada nota.
/// </para>
///
/// <para>
/// El promedio final del curso es el promedio simple de los 3 periodos cerrados.
/// </para>
/// </summary>
public static class PromedioCalculator
{
    /// <summary>
    /// Calcula el promedio ponderado de un estudiante en un periodo.
    /// Fórmula: <c>Σ (nota_i × porcentaje_i) / 100</c>, donde <c>porcentaje_i</c>
    /// proviene de la actividad asociada a la calificación.
    /// Devuelve <c>null</c> si no hay calificaciones o la suma de porcentajes es 0.
    /// </summary>
    public static decimal? PromedioPeriodo(
        IEnumerable<Calificacion> calificaciones,
        IReadOnlyDictionary<long, Actividad> actividadesPorId)
    {
        var lista = calificaciones?.ToList() ?? new List<Calificacion>();
        if (lista.Count == 0) return null;

        decimal sumaPorcentajes = 0m;
        decimal sumaPonderada = 0m;

        foreach (var c in lista)
        {
            if (!actividadesPorId.TryGetValue(c.ActividadId, out var act)) continue;
            sumaPorcentajes += act.Porcentaje;
            sumaPonderada += c.Nota * act.Porcentaje;
        }

        if (sumaPorcentajes == 0) return null;
        return Math.Round(sumaPonderada / 100m, 2);
    }

    /// <summary>
    /// Calcula el promedio final del curso a partir de los promedios de los 3 periodos.
    /// Solo válido cuando los tres periodos están cerrados y tienen promedio calculado.
    /// </summary>
    public static decimal? PromedioFinalCurso(decimal? p1, decimal? p2, decimal? p3)
    {
        if (p1 is null || p2 is null || p3 is null) return null;
        return Math.Round((p1.Value + p2.Value + p3.Value) / 3m, 2);
    }

    /// <summary>
    /// Suma de los porcentajes de las actividades activas de un curso,
    /// excluyendo opcionalmente una actividad (por ejemplo, la que se está editando).
    /// </summary>
    public static decimal SumaPorcentajesActividadesCurso(
        IEnumerable<Actividad> actividadesDelCurso,
        long? excluirId = null)
    {
        return actividadesDelCurso
            .Where(a => a.Activo && (excluirId is null || a.Id != excluirId))
            .Sum(a => a.Porcentaje);
    }
}
