#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using Sac.Domain.Enums;
using Sac.Domain.Events;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Periodo Académico (RF-06, HU-10). Cada Curso tiene 3 periodos: Corte 1, 2 y 3.
/// Reglas:
///  - Estado: Abierto / Cerrado.
///  - Solo el Director puede cerrar un periodo.
///  - Una vez cerrado, no se permiten cambios en actividades ni notas (RF-06, RF-07, RF-08).
/// </summary>
public sealed class Periodo : Aggregate<long>
{
    private Periodo() { }

    public long CursoId { get; private set; }
    public NumeroCorte NumeroCorte { get; private set; }
    public EstadoPeriodo Estado { get; private set; }
    public DateTime? FechaCierre { get; private set; }

    /// <summary>
    /// Promedio del periodo calculado en función de las calificaciones registradas.
    /// Se actualiza automáticamente cuando se cierra el periodo o cuando se calcula promedios.
    /// </summary>
    public decimal? PromedioPeriodo { get; private set; }

    public static Periodo Create(long cursoId, NumeroCorte numeroCorte)
    {
        if (cursoId <= 0)
            throw new DomainException("El curso es obligatorio.", nameof(Periodo));

        return new Periodo
        {
            CursoId = cursoId,
            NumeroCorte = numeroCorte,
            Estado = EstadoPeriodo.Abierto,
            FechaCierre = null,
            PromedioPeriodo = null
        };
    }

    /// <summary>Cierra el periodo. Solo el Director debe invocar esto (RF-06).</summary>
    public void Cerrar()
    {
        if (Estado == EstadoPeriodo.Cerrado)
            throw new DomainException("El periodo ya se encuentra cerrado.", nameof(Periodo));

        Estado = EstadoPeriodo.Cerrado;
        FechaCierre = BuildingBlocks.Source.Application.Utils.AppDateTime.Now;

        AddDomainEvent(new PeriodoCerradoEvent(Id, CursoId));
    }

    /// <summary>
    /// Reabre el periodo (autorización explícita del Director, RF-06).
    /// </summary>
    public void Reabrir()
    {
        if (Estado == EstadoPeriodo.Abierto)
            throw new DomainException("El periodo ya se encuentra abierto.", nameof(Periodo));

        Estado = EstadoPeriodo.Abierto;
        FechaCierre = null;
    }

    /// <summary>Actualiza el promedio del periodo (RF-09).</summary>
    public void ActualizarPromedio(decimal promedio)
    {
        if (promedio is < 0 or > 5)
            throw new DomainException("El promedio del periodo debe estar entre 0.0 y 5.0.", nameof(Periodo));

        PromedioPeriodo = Math.Round(promedio, 2);
    }

    public bool EstaCerrado => Estado == EstadoPeriodo.Cerrado;
    public bool EstaAbierto => Estado == EstadoPeriodo.Abierto;
}
