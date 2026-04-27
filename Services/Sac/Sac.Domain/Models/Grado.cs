#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Grado (RF-03, HU-04). Catálogo maestro administrado por el Director.
/// Ejemplos: 6-1, 6-2, 7-1.
/// </summary>
public sealed class Grado : Aggregate<long>
{
    private Grado() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }

    public static Grado Create(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del grado es obligatorio.", nameof(Grado));

        if (nombre.Length > 50)
            throw new DomainException("El nombre del grado no puede superar 50 caracteres.", nameof(Grado));

        return new Grado
        {
            Nombre = nombre.Trim().ToUpperInvariant(),
            Descripcion = descripcion?.Trim(),
            Activo = true
        };
    }

    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del grado es obligatorio.", nameof(Grado));

        Nombre = nombre.Trim().ToUpperInvariant();
        Descripcion = descripcion?.Trim();
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
