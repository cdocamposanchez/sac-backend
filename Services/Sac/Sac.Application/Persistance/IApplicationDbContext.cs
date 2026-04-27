#region

using Sac.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace Sac.Application.Persistance;

public interface IApplicationDbContext
{
    DbSet<Director> Directores { get; }
    DbSet<Docente> Docentes { get; }
    DbSet<Estudiante> Estudiantes { get; }
    DbSet<Grado> Grados { get; }
    DbSet<Materia> Materias { get; }
    DbSet<Curso> Cursos { get; }
    DbSet<CursoEstudiante> CursoEstudiantes { get; }
    DbSet<Periodo> Periodos { get; }
    DbSet<Actividad> Actividades { get; }
    DbSet<Calificacion> Calificaciones { get; }
    DbSet<EnvioBoletin> EnviosBoletin { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
