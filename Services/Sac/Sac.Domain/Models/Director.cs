#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Sac.Domain.Enums;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz que representa al Director del colegio.
/// El Director es el único usuario con permisos para gestionar Docentes,
/// Estudiantes, Grados, Materias, Cursos, Asignaciones y Boletines (RF-02 a RF-06, RF-10, RF-11).
/// </summary>
public sealed class Director : Aggregate<long>
{
    private Director() { }

    public string NombreCompleto { get; private set; } = string.Empty;
    public string Cedula { get; private set; } = string.Empty;
    public string Correo { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool Activo { get; private set; }
    public static string Rol => RolUsuario.Director;

    /// <summary>
    /// Crea una instancia del Director aplicando reglas de dominio:
    /// nombre, cédula y correo no vacíos; cédula numérica; correo válido; contraseña hasheada.
    /// </summary>
    public static Director Create(string nombreCompleto, string cedula, string correo, string password)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new DomainException("El nombre completo es obligatorio.", nameof(Director));

        if (string.IsNullOrWhiteSpace(cedula))
            throw new DomainException("La cédula es obligatoria.", nameof(Director));

        if (!cedula.All(char.IsDigit))
            throw new DomainException("La cédula debe contener únicamente dígitos.", nameof(Director));

        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new DomainException("El correo electrónico no tiene un formato válido.", nameof(Director));

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new DomainException("La contraseña debe tener al menos 6 caracteres.", nameof(Director));

        return new Director
        {
            NombreCompleto = nombreCompleto.Trim(),
            Cedula = cedula.Trim(),
            Correo = correo.Trim().ToLowerInvariant(),
            PasswordHash = PasswordHasher.Hash(password),
            Activo = true
        };
    }

    public bool VerificarPassword(string plainPassword) =>
        PasswordHasher.Verify(plainPassword, PasswordHash);

    public void CambiarPassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            throw new DomainException("La nueva contraseña debe tener al menos 6 caracteres.", nameof(Director));

        PasswordHash = PasswordHasher.Hash(newPassword);
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
