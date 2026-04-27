namespace Sac.Domain.Errors;

public enum CodigosError
{
    // Éxito
    TransaccionExitosa = 0000,

    // Errores generales
    TransaccionFallida = 1000,
    DatoNoPuedeSerNulo = 1001,
    ValorNoValido = 1002,
    FormatoNoValido = 1003,
    CampoObligatorioAusente = 1004,
    LongitudNoPermitida = 1005,
    DatoNoExiste = 1006,
    EstructuraIncompleta = 1008,
    DatoDuplicado = 1010,

    // Autenticación y autorización
    UsuarioYClaveNoValidos = 2001,
    UsuarioNoPermitido = 2002,
    UsuarioNoRegistrado = 2003,
    SesionExpirada = 2004,

    // Reglas de negocio - Calificaciones
    NotaFueraDeRango = 3001,
    PeriodoCerrado = 3002,
    CursoCerrado = 3003,
    PorcentajeExcedido = 3004,
    DocenteNoAsignado = 3005,
    EstudianteNoEnCurso = 3006,
    PeriodosPendientesCierre = 3007,
    CursoNoCerrado = 3008,

    // Boletines
    BoletinEnvioFallido = 4001
}

public sealed record Error(
    CodigosError Codigo,
    string Descripcion,
    string? DetalleTecnico = null
);
