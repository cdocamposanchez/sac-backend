#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Sac.Domain.Enums;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz Docente. Implementa RF-02 (gestión por Director) y RF-01 (autenticación por cédula).
/// Un Docente puede ser asignado a múltiples Cursos, pero cada Curso solo tiene UN Docente (RF-04).
/// </summary>
public sealed class Docente : Aggregate<long>
{
    private Docente() { }

    public string NombreCompleto { get; private set; } = string.Empty;
    public string Cedula { get; private set; } = string.Empty;
    public string Correo { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool Activo { get; private set; }
    public static string Rol => RolUsuario.Docente;

    /// <summary>
    /// Crea un Docente aplicando reglas de dominio: cédula numérica, correo con formato válido,
    /// contraseña con mínimo 6 caracteres, hash con BCrypt.
    /// </summary>
    public static Docente Create(string nombreCompleto, string cedula, string correo, string password)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new DomainException("El nombre completo es obligatorio.", nameof(Docente));

        if (string.IsNullOrWhiteSpace(cedula))
            throw new DomainException("La cédula es obligatoria.", nameof(Docente));

        if (!cedula.All(char.IsDigit))
            throw new DomainException("La cédula debe contener únicamente dígitos.", nameof(Docente));

        if (cedula.Length is < 6 or > 15)
            throw new DomainException("La cédula debe tener entre 6 y 15 dígitos.", nameof(Docente));

        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DomainException("El correo electrónico no tiene un formato válido.", nameof(Docente));

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new DomainException("La contraseña debe tener al menos 6 caracteres.", nameof(Docente));

        return new Docente
        {
            NombreCompleto = nombreCompleto.Trim(),
            Cedula = cedula.Trim(),
            Correo = correo.Trim().ToLowerInvariant(),
            PasswordHash = PasswordHasher.Hash(password),
            Activo = true
        };
    }

    /// <summary>Permite editar el nombre y el correo (RF-02). La cédula es inmutable.</summary>
    public void Actualizar(string nombreCompleto, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new DomainException("El nombre completo es obligatorio.", nameof(Docente));

        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DomainException("El correo electrónico no tiene un formato válido.", nameof(Docente));

        NombreCompleto = nombreCompleto.Trim();
        Correo = correo.Trim().ToLowerInvariant();
    }

    public bool VerificarPassword(string plainPassword) =>
        PasswordHasher.Verify(plainPassword, PasswordHash);

    public void CambiarPassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new DomainException("La nueva contraseña debe tener al menos 6 caracteres.", nameof(Docente));

        PasswordHash = PasswordHasher.Hash(newPassword);
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
