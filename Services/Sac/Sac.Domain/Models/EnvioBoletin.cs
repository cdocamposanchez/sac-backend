#region

using BuildingBlocks.Source.Domain.Abstractions;
using BuildingBlocks.Source.Domain.Exception;
using Sac.Domain.Enums;

#endregion

namespace Sac.Domain.Models;

/// <summary>
/// Agregado raíz EnvioBoletin (RF-11, HU-11). Registro del envío individual de un boletín
/// final por correo a un estudiante. Solo se permite cuando el Curso está CERRADO.
/// El sistema lleva trazabilidad de éxito/fallo y permite reintentos en caso de error.
/// </summary>
public sealed class EnvioBoletin : Aggregate<long>
{
    private EnvioBoletin() { }

    public long CursoId { get; private set; }
    public long EstudianteId { get; private set; }
    public string CorreoDestino { get; private set; } = string.Empty;
    public EstadoEnvioBoletin Estado { get; private set; }
    public DateTime? FechaEnvio { get; private set; }
    public string? MotivoFallo { get; private set; }
    public int Intentos { get; private set; }

    public static EnvioBoletin Create(long cursoId, long estudianteId, string correoDestino)
    {
        if (cursoId <= 0)
            throw new DomainException("El curso es obligatorio.", nameof(EnvioBoletin));

        if (estudianteId <= 0)
            throw new DomainException("El estudiante es obligatorio.", nameof(EnvioBoletin));

        if (string.IsNullOrWhiteSpace(correoDestino) || !correoDestino.Contains('@'))
            throw new DomainException("El correo destino no tiene un formato válido.", nameof(EnvioBoletin));

        return new EnvioBoletin
        {
            CursoId = cursoId,
            EstudianteId = estudianteId,
            CorreoDestino = correoDestino.Trim().ToLowerInvariant(),
            Estado = EstadoEnvioBoletin.Pendiente,
            Intentos = 0
        };
    }

    public void MarcarComoEnviado()
    {
        Estado = EstadoEnvioBoletin.Enviado;
        FechaEnvio = BuildingBlocks.Source.Application.Utils.AppDateTime.Now;
        MotivoFallo = null;
        Intentos++;
    }

    public void MarcarComoFallido(string motivo)
    {
        Estado = EstadoEnvioBoletin.Fallido;
        MotivoFallo = motivo;
        Intentos++;
    }

    /// <summary>Vuelve a poner el envío en cola para reintento.</summary>
    public void EncolarReintento()
    {
        if (Estado == EstadoEnvioBoletin.Enviado)
            throw new DomainException("No se puede reintentar un envío ya completado.", nameof(EnvioBoletin));

        Estado = EstadoEnvioBoletin.Pendiente;
    }
}
