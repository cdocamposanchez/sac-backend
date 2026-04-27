#region

using Sac.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace Sac.Infrastructure.Persistance.Configuration;

// ====== Director ======
public class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> b)
    {
        b.ToTable("directores");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.NombreCompleto).HasColumnName("t_nombre_completo").HasMaxLength(150).IsRequired();
        b.Property(x => x.Cedula).HasColumnName("t_cedula").HasMaxLength(15).IsRequired();
        b.Property(x => x.Correo).HasColumnName("t_correo").HasMaxLength(150).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("t_password_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.Cedula).IsUnique();
        b.HasIndex(x => x.Correo).IsUnique();
    }
}

// ====== Docente ======
public class DocenteConfiguration : IEntityTypeConfiguration<Docente>
{
    public void Configure(EntityTypeBuilder<Docente> b)
    {
        b.ToTable("docentes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.NombreCompleto).HasColumnName("t_nombre_completo").HasMaxLength(150).IsRequired();
        b.Property(x => x.Cedula).HasColumnName("t_cedula").HasMaxLength(15).IsRequired();
        b.Property(x => x.Correo).HasColumnName("t_correo").HasMaxLength(150).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("t_password_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.Cedula).IsUnique();
        b.HasIndex(x => x.Correo).IsUnique();
    }
}

// ====== Estudiante ======
public class EstudianteConfiguration : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(EntityTypeBuilder<Estudiante> b)
    {
        b.ToTable("estudiantes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.NombreCompleto).HasColumnName("t_nombre_completo").HasMaxLength(150).IsRequired();
        b.Property(x => x.Cedula).HasColumnName("t_cedula").HasMaxLength(15).IsRequired();
        b.Property(x => x.Correo).HasColumnName("t_correo").HasMaxLength(150).IsRequired();
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.Cedula).IsUnique();
        b.HasIndex(x => x.Correo).IsUnique();
    }
}

// ====== Grado ======
public class GradoConfiguration : IEntityTypeConfiguration<Grado>
{
    public void Configure(EntityTypeBuilder<Grado> b)
    {
        b.ToTable("grados");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.Nombre).HasColumnName("t_nombre").HasMaxLength(50).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("t_descripcion").HasMaxLength(255);
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.Nombre).IsUnique();
    }
}

// ====== Materia ======
public class MateriaConfiguration : IEntityTypeConfiguration<Materia>
{
    public void Configure(EntityTypeBuilder<Materia> b)
    {
        b.ToTable("materias");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("n_id").ValueGeneratedOnAdd();
        b.Property(x => x.Nombre).HasColumnName("t_nombre").HasMaxLength(100).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("t_descripcion").HasMaxLength(255);
        b.Property(x => x.Activo).HasColumnName("b_activo").IsRequired();

        b.HasIndex(x => x.Nombre).IsUnique();
    }
}
