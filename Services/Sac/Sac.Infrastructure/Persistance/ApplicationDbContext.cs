#region

using System.Reflection;
using Sac.Application.Persistance;
using Sac.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace Sac.Infrastructure.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Director> Directores => Set<Director>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Grado> Grados => Set<Grado>();
    public DbSet<Materia> Materias => Set<Materia>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<CursoEstudiante> CursoEstudiantes => Set<CursoEstudiante>();
    public DbSet<Periodo> Periodos => Set<Periodo>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    public DbSet<EnvioBoletin> EnviosBoletin => Set<EnvioBoletin>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema(Environment.GetEnvironmentVariable("CALIFICACIONES_DB_SCHEMA") ?? "public");
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Convención global: nombres de columnas para FechaReg/FechaMod
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var propFechaReg = entityType.FindProperty("FechaReg");
            var propFechaMod = entityType.FindProperty("FechaMod");

            if (propFechaReg != null)
            {
                propFechaReg.SetColumnName("fecha_reg");
                propFechaReg.SetDefaultValueSql("CURRENT_TIMESTAMP");
            }

            propFechaMod?.SetColumnName("fecha_mod");
        }

        base.OnModelCreating(modelBuilder);
    }
}
