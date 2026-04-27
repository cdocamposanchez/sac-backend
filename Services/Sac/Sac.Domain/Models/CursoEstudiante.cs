#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado CursoEstudiante (RF-05, HU-07). Tabla intermedia que vincula
/// a un estudiante con un curso. Solo el Director puede crear/editar/eliminar
/// estas asignaciones. Un estudiante puede pertenecer a varios cursos.
/// </summary>
public sealed class CursoEstudiante : Aggregate<long>
{
    private CursoEstudiante() { }

    public long CursoId { get; private set; }
    public long EstudianteId { get; private set; }
    public DateTime FechaAsignacion { get; private set; }
    public bool Activo { get; private set; }

    public static CursoEstudiante Create(long cursoId, long estudianteId)
    {
        if (cursoId <= 0)
            throw new DomainException("El curso es obligatorio.", nameof(CursoEstudiante));

        if (estudianteId <= 0)
            throw new DomainException("El estudiante es obligatorio.", nameof(CursoEstudiante));

        return new CursoEstudiante
        {
            CursoId = cursoId,
            EstudianteId = estudianteId,
            FechaAsignacion = BuildingBlocks.Source.Application.Utils.AppDateTime.Now,
            Activo = true
        };
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
