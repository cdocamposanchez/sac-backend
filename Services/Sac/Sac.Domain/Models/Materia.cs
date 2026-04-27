#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Materia (RF-03, HU-05). Catálogo maestro administrado por el Director.
/// Ejemplos: Matemáticas, Español, Ciencias, Inglés.
/// </summary>
public sealed class Materia : Aggregate<long>
{
    private Materia() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }

    public static Materia Create(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la materia es obligatorio.", nameof(Materia));

        if (nombre.Length > 100)
            throw new DomainException("El nombre de la materia no puede superar 100 caracteres.", nameof(Materia));

        return new Materia
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Activo = true
        };
    }

    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la materia es obligatorio.", nameof(Materia));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
