#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using Sac.Domain.Enums;
using Sac.Domain.Events;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Curso (RF-04, HU-06). Combina Grado + Materia + Docente.
/// Reglas:
///  - Solo el Director puede crearlo, editarlo y cerrarlo.
///  - Tiene exactamente UN docente asignado.
///  - Estado inicial: Activo. Solo puede cerrarse cuando los 3 periodos estén cerrados (RF-04).
///  - Una vez cerrado, no se permiten modificaciones de actividades ni notas (RF-06, RF-08).
/// </summary>
public sealed class Curso : Aggregate<long>
{
    private Curso() { }

    public string Nombre { get; private set; } = string.Empty;
    public long GradoId { get; private set; }
    public long MateriaId { get; private set; }
    public long DocenteId { get; private set; }
    public EstadoCurso Estado { get; private set; }

    /// <summary>Año académico al que pertenece el curso (ej: 2026).</summary>
    public int AnioAcademico { get; private set; }

    public static Curso Create(string nombre, long gradoId, long materiaId, long docenteId, int anioAcademico)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del curso es obligatorio.", nameof(Curso));

        if (gradoId <= 0)
            throw new DomainException("El grado es obligatorio.", nameof(Curso));

        if (materiaId <= 0)
            throw new DomainException("La materia es obligatoria.", nameof(Curso));

        if (docenteId <= 0)
            throw new DomainException("El docente es obligatorio. Un curso debe tener exactamente un docente.", nameof(Curso));

        if (anioAcademico < 2000 || anioAcademico > 2100)
            throw new DomainException("El año académico no es válido.", nameof(Curso));

        return new Curso
        {
            Nombre = nombre.Trim(),
            GradoId = gradoId,
            MateriaId = materiaId,
            DocenteId = docenteId,
            AnioAcademico = anioAcademico,
            Estado = EstadoCurso.Activo
        };
    }

    /// <summary>Permite editar nombre, grado, materia y docente solo si el curso está Activo.</summary>
    public void Actualizar(string nombre, long gradoId, long materiaId, long docenteId)
    {
        if (Estado == EstadoCurso.Cerrado)
            throw new DomainException("No se puede modificar un curso cerrado.", nameof(Curso));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del curso es obligatorio.", nameof(Curso));

        if (docenteId <= 0)
            throw new DomainException("El docente es obligatorio.", nameof(Curso));

        Nombre = nombre.Trim();
        GradoId = gradoId;
        MateriaId = materiaId;
        DocenteId = docenteId;
    }

    /// <summary>
    /// Cierra el curso. Solo se permite cuando los TRES periodos están cerrados (RF-04, RF-10).
    /// </summary>
    public void Cerrar(IReadOnlyCollection<Periodo> periodosDelCurso)
    {
        if (Estado == EstadoCurso.Cerrado)
            throw new DomainException("El curso ya se encuentra cerrado.", nameof(Curso));

        if (periodosDelCurso.Count < 3)
            throw new DomainException(
                "El curso debe tener sus tres periodos creados antes de poder cerrarse.",
                nameof(Curso));

        if (periodosDelCurso.Any(p => p.Estado != EstadoPeriodo.Cerrado))
            throw new DomainException(
                "No se puede cerrar el curso: existen periodos abiertos. Cierre primero los tres periodos.",
                nameof(Curso));

        Estado = EstadoCurso.Cerrado;
        AddDomainEvent(new CursoCerradoEvent(Id));
    }

    public bool EstaCerrado => Estado == EstadoCurso.Cerrado;
    public bool EstaActivo => Estado == EstadoCurso.Activo;

    /// <summary>Verifica si el docente indicado es el dueño del curso.</summary>
    public bool PerteneceADocente(long docenteId) => DocenteId == docenteId;
}
