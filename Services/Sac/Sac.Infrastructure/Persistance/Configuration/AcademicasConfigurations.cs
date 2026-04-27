#region

using Sac.Domain.Enums;
using Sac.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Sac.Infrastructure.Persistance.Configuration;

// ====== Curso ======
public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> b)
    {
        b.ToTable("cursos");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.Nombre).HasColumnName("t_nombre").HasMaxLength(150).IsRequired();
        b.Property(x => x.GradoId).HasColumnName("n_id_grado").IsRequired();
        b.Property(x => x.MateriaId).HasColumnName("n_id_materia").IsRequired();
        b.Property(x => x.DocenteId).HasColumnName("n_id_docente").IsRequired();
        b.Property(x => x.AnioAcademico).HasColumnName("n_anio_academico").IsRequired();
        b.Property(x => x.Estado).HasColumnName("n_estado").HasConversion<int>().IsRequired();

        b.HasIndex(x => new { x.GradoId, x.MateriaId, x.AnioAcademico }).IsUnique();

        b.HasOne<Grado>().WithMany().HasForeignKey(x => x.GradoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Materia>().WithMany().HasForeignKey(x => x.MateriaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Docente>().WithMany().HasForeignKey(x => x.DocenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ====== CursoEstudiante ======
public class CursoEstudianteConfiguration : IEntityTypeConfiguration<CursoEstudiante>
{
    public void Configure(EntityTypeBuilder<CursoEstudiante> b)
    {
        b.ToTable("curso_estudiante");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.CursoId).HasColumnName("n_id_curso").IsRequired();
        b.Property(x => x.EstudianteId).HasColumnName("n_id_estudiante").IsRequired();
        b.Property(x => x.FechaAsignacion).HasColumnName("d_fecha_asignacion").IsRequired();
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => new { x.CursoId, x.EstudianteId }).IsUnique();

        b.HasOne<Curso>().WithMany().HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Estudiante>().WithMany().HasForeignKey(x => x.EstudianteId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ====== Periodo ======
public class PeriodoConfiguration : IEntityTypeConfiguration<Periodo>
{
    public void Configure(EntityTypeBuilder<Periodo> b)
    {
        b.ToTable("periodos");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.CursoId).HasColumnName("n_id_curso").IsRequired();
        b.Property(x => x.NumeroCorte).HasColumnName("n_numero_corte").HasConversion<int>().IsRequired();
        b.Property(x => x.Estado).HasColumnName("n_estado").HasConversion<int>().IsRequired();
        b.Property(x => x.FechaCierre).HasColumnName("d_fecha_cierre");
        b.Property(x => x.PromedioPeriodo).HasColumnName("n_promedio").HasColumnType("numeric(5,2)");

        b.HasIndex(x => new { x.CursoId, x.NumeroCorte }).IsUnique();

        b.HasOne<Curso>().WithMany().HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ====== Actividad ======
public class ActividadConfiguration : IEntityTypeConfiguration<Actividad>
{
    public void Configure(EntityTypeBuilder<Actividad> b)
    {
        b.ToTable("actividades");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.Nombre).HasColumnName("t_nombre").HasMaxLength(200).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("t_descripcion").HasMaxLength(500);
        b.Property(x => x.CursoId).HasColumnName("n_id_curso").IsRequired();
        b.Property(x => x.PeriodoId).HasColumnName("n_id_periodo").IsRequired();
        b.Property(x => x.Porcentaje).HasColumnName("n_porcentaje").HasColumnType("numeric(5,2)").IsRequired();
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.CursoId);
        b.HasOne<Curso>().WithMany().HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Periodo>().WithMany().HasForeignKey(x => x.PeriodoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ====== Calificacion ======
public class CalificacionConfiguration : IEntityTypeConfiguration<Calificacion>
{
    public void Configure(EntityTypeBuilder<Calificacion> b)
    {
        b.ToTable("calificaciones");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.EstudianteId).HasColumnName("n_id_estudiante").IsRequired();
        b.Property(x => x.ActividadId).HasColumnName("n_id_actividad").IsRequired();
        b.Property(x => x.CursoId).HasColumnName("n_id_curso").IsRequired();
        b.Property(x => x.PeriodoId).HasColumnName("n_id_periodo").IsRequired();
        b.Property(x => x.Nota).HasColumnName("n_nota").HasColumnType("numeric(3,2)").IsRequired();

        // RNF-05: nota única por estudiante+actividad
        b.HasIndex(x => new { x.EstudianteId, x.ActividadId }).IsUnique();
        b.HasIndex(x => new { x.CursoId, x.PeriodoId });

        b.HasOne<Estudiante>().WithMany().HasForeignKey(x => x.EstudianteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Actividad>().WithMany().HasForeignKey(x => x.ActividadId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Curso>().WithMany().HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Periodo>().WithMany().HasForeignKey(x => x.PeriodoId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ====== EnvioBoletin ======
public class EnvioBoletinConfiguration : IEntityTypeConfiguration<EnvioBoletin>
{
    public void Configure(EntityTypeBuilder<EnvioBoletin> b)
    {
        b.ToTable("envios_boletin");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.CursoId).HasColumnName("n_id_curso").IsRequired();
        b.Property(x => x.EstudianteId).HasColumnName("n_id_estudiante").IsRequired();
        b.Property(x => x.CorreoDestino).HasColumnName("t_correo_destino").HasMaxLength(150).IsRequired();
        b.Property(x => x.Estado).HasColumnName("n_estado").HasConversion<int>().IsRequired();
        b.Property(x => x.FechaEnvio).HasColumnName("d_fecha_envio");
        b.Property(x => x.MotivoFallo).HasColumnName("t_motivo_fallo").HasMaxLength(1000);
        b.Property(x => x.Intentos).HasColumnName("n_intentos").IsRequired();

        b.HasOne<Curso>().WithMany().HasForeignKey(x => x.CursoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Estudiante>().WithMany().HasForeignKey(x => x.EstudianteId).OnDelete(DeleteBehavior.Cascade);
    }
}
