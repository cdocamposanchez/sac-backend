#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Estudiante (RF-02, HU-03). El Estudiante NO inicia sesión en el sistema;
/// su correo se usa exclusivamente para envío de boletines (RF-11).
/// Solo el Director puede gestionarlos.
/// </summary>
public sealed class Estudiante : Aggregate<long>
{
    private Estudiante() { }

    public string NombreCompleto { get; private set; } = string.Empty;
    public string Cedula { get; private set; } = string.Empty;
    public string Correo { get; private set; } = string.Empty;
    public bool Activo { get; private set; }

    public static Estudiante Create(string nombreCompleto, string cedula, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new DomainException("El nombre completo es obligatorio.", nameof(Estudiante));

        if (string.IsNullOrWhiteSpace(cedula))
            throw new DomainException("La cédula es obligatoria.", nameof(Estudiante));

        if (!cedula.All(char.IsDigit))
            throw new DomainException("La cédula debe contener únicamente dígitos.", nameof(Estudiante));

        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DomainException("El correo electrónico no tiene un formato válido.", nameof(Estudiante));

        return new Estudiante
        {
            NombreCompleto = nombreCompleto.Trim(),
            Cedula = cedula.Trim(),
            Correo = correo.Trim().ToLowerInvariant(),
            Activo = true
        };
    }

    public void Actualizar(string nombreCompleto, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new DomainException("El nombre completo es obligatorio.", nameof(Estudiante));

        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DomainException("El correo electrónico no tiene un formato válido.", nameof(Estudiante));

        NombreCompleto = nombreCompleto.Trim();
        Correo = correo.Trim().ToLowerInvariant();
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
