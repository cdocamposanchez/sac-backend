#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Actividad (RF-07, RF-08, HU-08). Una actividad pertenece a un Curso y a un Periodo.
///
/// <para><b>Reglas:</b></para>
/// <list type="bullet">
///   <item>Solo el Docente del curso puede crearla, editarla o eliminarla.</item>
///   <item>Solo se puede crear/editar si el periodo está Abierto (RF-07).</item>
///   <item>La cantidad de actividades por periodo es ilimitada.</item>
///   <item>
///     Cada actividad tiene un <see cref="Porcentaje"/> entre <see cref="PorcentajeMinimo"/>
///     (1%) y <see cref="PorcentajeMaximo"/> (100%). La suma de los porcentajes de todas las
///     actividades activas de un curso no puede superar 100% (validado en
///     <see cref="ValidarSumaPorcentajesCurso"/>, RF-08).
///   </item>
/// </list>
/// </summary>
public sealed class Actividad : Aggregate<long>
{
    public const decimal PorcentajeMinimo = 1m;
    public const decimal PorcentajeMaximo = 100m;

    private Actividad() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public long CursoId { get; private set; }
    public long PeriodoId { get; private set; }
    public decimal Porcentaje { get; private set; }
    public bool Activo { get; private set; }

    public static Actividad Create(
        string nombre,
        string? descripcion,
        long cursoId,
        long periodoId,
        decimal porcentaje)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la actividad es obligatorio.", nameof(Actividad));

        if (nombre.Length > 200)
            throw new DomainException("El nombre de la actividad no puede superar 200 caracteres.", nameof(Actividad));

        if (cursoId <= 0)
            throw new DomainException("El curso es obligatorio.", nameof(Actividad));

        if (periodoId <= 0)
            throw new DomainException("El periodo es obligatorio.", nameof(Actividad));

        ValidarPorcentaje(porcentaje);

        return new Actividad
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            CursoId = cursoId,
            PeriodoId = periodoId,
            Porcentaje = Math.Round(porcentaje, 2),
            Activo = true
        };
    }

    public void Actualizar(string nombre, string? descripcion, decimal porcentaje)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la actividad es obligatorio.", nameof(Actividad));

        ValidarPorcentaje(porcentaje);

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        Porcentaje = Math.Round(porcentaje, 2);
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;

    // ====== Validaciones ======

    private static void ValidarPorcentaje(decimal porcentaje)
    {
        if (porcentaje < PorcentajeMinimo || porcentaje > PorcentajeMaximo)
            throw new DomainException(
                $"El porcentaje de la actividad debe estar entre {PorcentajeMinimo:0.##}% y {PorcentajeMaximo:0.##}%.",
                nameof(Actividad));
    }

    /// <summary>
    /// Validación de dominio aplicada por el handler antes de persistir:
    /// la suma de los porcentajes de las actividades activas del curso (excluyendo esta misma)
    /// más el nuevo porcentaje no puede superar el 100% (RF-08).
    /// </summary>
    public static void ValidarSumaPorcentajesCurso(decimal sumaActual, decimal nuevoPorcentaje)
    {
        var total = sumaActual + nuevoPorcentaje;
        if (total > PorcentajeMaximo)
        {
            var disponible = PorcentajeMaximo - sumaActual;
            throw new DomainException(
                $"La suma de porcentajes de las actividades del curso excede el {PorcentajeMaximo:0.##}%. " +
                $"Disponible: {disponible:0.##}%. Intentaste asignar {nuevoPorcentaje:0.##}%.",
                nameof(Actividad));
        }
    }
}
