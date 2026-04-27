#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using Sac.Domain.Events;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Calificación (RF-08, RF-09, HU-09). Asocia una nota a un Estudiante,
/// Actividad, Curso y Periodo.
///
/// <para><b>Reglas de negocio (validadas en este agregado):</b></para>
/// <list type="bullet">
///   <item>Nota válida: rango [1.0 - 5.0] (RF-08, RNF-05).</item>
///   <item>Modificación/eliminación SOLO si el periodo está Abierto (RF-08).</item>
///   <item>Solo el Docente asignado al curso puede registrar/editar/eliminar (RF-08).</item>
/// </list>
///
/// <para>
/// El <b>porcentaje</b> ya no vive en la calificación: vive en la
/// <see cref="Actividad"/>. El aporte ponderado de una nota al promedio del periodo
/// se calcula como <c>nota × actividad.Porcentaje / 100</c> en
/// <see cref="AportePonderado(decimal)"/> (RF-09).
/// </para>
/// </summary>
public sealed class Calificacion : Aggregate<long>
{
    public const decimal NotaMinima = 1.0m;
    public const decimal NotaMaxima = 5.0m;

    private Calificacion() { }

    public long EstudianteId { get; private set; }
    public long ActividadId { get; private set; }
    public long CursoId { get; private set; }
    public long PeriodoId { get; private set; }
    public decimal Nota { get; private set; }

    /// <summary>Crea una calificación validando rango [1.0 - 5.0].</summary>
    public static Calificacion Create(
        long estudianteId,
        long actividadId,
        long cursoId,
        long periodoId,
        decimal nota)
    {
        ValidarIds(estudianteId, actividadId, cursoId, periodoId);
        ValidarNota(nota);

        var calificacion = new Calificacion
        {
            EstudianteId = estudianteId,
            ActividadId = actividadId,
            CursoId = cursoId,
            PeriodoId = periodoId,
            Nota = Math.Round(nota, 2)
        };

        calificacion.AddDomainEvent(new CalificacionRegistradaEvent(
            calificacion.Id, estudianteId, cursoId, periodoId, calificacion.Nota));

        return calificacion;
    }

    /// <summary>Modifica la nota. Solo válido si el periodo está abierto.</summary>
    public void Actualizar(decimal nota, bool periodoAbierto)
    {
        if (!periodoAbierto)
            throw new DomainException(
                "No se pueden modificar calificaciones en un periodo cerrado.",
                nameof(Calificacion));

        ValidarNota(nota);
        Nota = Math.Round(nota, 2);
    }

    /// <summary>Indica si se puede eliminar (regla: solo si el periodo está abierto).</summary>
    public bool PuedeEliminarse(bool periodoAbierto) => periodoAbierto;

    /// <summary>
    /// Cálculo del aporte ponderado al promedio del periodo: <c>nota × porcentajeActividad / 100</c>.
    /// El porcentaje viene desde la <see cref="Actividad"/> asociada (RF-09).
    /// </summary>
    public decimal AportePonderado(decimal porcentajeActividad)
        => Math.Round(Nota * porcentajeActividad / 100m, 4);

    // ====== Validaciones ======

    private static void ValidarIds(long estudianteId, long actividadId, long cursoId, long periodoId)
    {
        if (estudianteId <= 0)
            throw new DomainException("El estudiante es obligatorio.", nameof(Calificacion));

        if (actividadId <= 0)
            throw new DomainException("La actividad es obligatoria.", nameof(Calificacion));

        if (cursoId <= 0)
            throw new DomainException("El curso es obligatorio.", nameof(Calificacion));

        if (periodoId <= 0)
            throw new DomainException("El periodo es obligatorio.", nameof(Calificacion));
    }

    private static void ValidarNota(decimal nota)
    {
        if (nota is < NotaMinima or > NotaMaxima)
            throw new DomainException(
                $"La nota debe estar entre {NotaMinima:0.0} y {NotaMaxima:0.0}.",
                nameof(Calificacion));
    }
}
