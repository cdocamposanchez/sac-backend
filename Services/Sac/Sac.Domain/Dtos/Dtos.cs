namespace Sac.Domain.Dtos;

// ====== Auth ======
public record LoginRequestDto(string Cedula, string Password);

public record LoginResponseDto(
    long UserId,
    string Cedula,
    string Nombre,
    string Correo,
    string Rol,
    string Token,
    DateTime ExpiraEn);

// ====== Director (perfil propio) ======
public record DirectorDto(
    long Id,
    string NombreCompleto,
    string Cedula,
    string Correo,
    bool Activo);

// ====== Docente ======
public record DocenteDto(
    long Id,
    string NombreCompleto,
    string Cedula,
    string Correo,
    bool Activo);

public record CreateDocenteDto(
    string NombreCompleto,
    string Cedula,
    string Correo,
    string Password);

public record UpdateDocenteDto(
    string NombreCompleto,
    string Correo);

// ====== Estudiante ======
public record EstudianteDto(
    long Id,
    string NombreCompleto,
    string Cedula,
    string Correo,
    bool Activo);

public record CreateEstudianteDto(
    string NombreCompleto,
    string Cedula,
    string Correo);

public record UpdateEstudianteDto(
    string NombreCompleto,
    string Correo);

// ====== Grado / Materia ======
public record GradoDto(long Id, string Nombre, string? Descripcion, bool Activo);
public record CreateGradoDto(string Nombre, string? Descripcion);

public record MateriaDto(long Id, string Nombre, string? Descripcion, bool Activo);
public record CreateMateriaDto(string Nombre, string? Descripcion);

// ====== Curso ======
public record CursoDto(
    long Id,
    string Nombre,
    long GradoId,
    string GradoNombre,
    long MateriaId,
    string MateriaNombre,
    long DocenteId,
    string DocenteNombre,
    int AnioAcademico,
    string Estado);

public record CreateCursoDto(
    string Nombre,
    long GradoId,
    long MateriaId,
    long DocenteId,
    int AnioAcademico);

public record UpdateCursoDto(
    string Nombre,
    long GradoId,
    long MateriaId,
    long DocenteId);

// ====== CursoEstudiante ======
public record CursoEstudianteDto(
    long Id,
    long CursoId,
    long EstudianteId,
    string EstudianteNombre,
    DateTime FechaAsignacion,
    bool Activo);

public record AsignarEstudianteDto(long CursoId, long EstudianteId);
public record AsignarEstudiantesLoteDto(long CursoId, List<long> EstudianteIds);

// ====== Periodo ======
public record PeriodoDto(
    long Id,
    long CursoId,
    int NumeroCorte,
    string Estado,
    DateTime? FechaCierre,
    decimal? PromedioPeriodo);

public record CreatePeriodoDto(long CursoId, int NumeroCorte);

// ====== Actividad ======
public record ActividadDto(
    long Id,
    string Nombre,
    string? Descripcion,
    long CursoId,
    long PeriodoId,
    decimal Porcentaje,
    bool Activo);

public record CreateActividadDto(
    string Nombre,
    string? Descripcion,
    long CursoId,
    long PeriodoId,
    decimal Porcentaje);

public record UpdateActividadDto(string Nombre, string? Descripcion, decimal Porcentaje);

// ====== Calificación ======
// El porcentaje vive en la actividad. Aquí se incluye como información derivada
// (ActividadPorcentaje) para que el frontend pueda mostrarlo sin un round-trip extra.
public record CalificacionDto(
    long Id,
    long EstudianteId,
    string? EstudianteNombre,
    long ActividadId,
    string? ActividadNombre,
    decimal ActividadPorcentaje,
    long CursoId,
    long PeriodoId,
    decimal Nota);

public record CreateCalificacionDto(
    long EstudianteId,
    long ActividadId,
    long CursoId,
    long PeriodoId,
    decimal Nota);

public record UpdateCalificacionDto(decimal Nota);

// ====== Reportes ======
public record ReporteEstudianteDto(
    long EstudianteId,
    string EstudianteNombre,
    string Cedula,
    string Correo,
    long CursoId,
    string CursoNombre,
    List<ReportePeriodoDto> Periodos,
    decimal? PromedioFinal);

public record ReportePeriodoDto(
    int NumeroCorte,
    string Estado,
    decimal? PromedioPeriodo,
    List<ReporteCalificacionDto> Calificaciones);

public record ReporteCalificacionDto(
    string ActividadNombre,
    decimal Nota,
    decimal Porcentaje);

// ====== Boletines ======
public record EnvioBoletinDto(
    long Id,
    long CursoId,
    long EstudianteId,
    string CorreoDestino,
    string Estado,
    DateTime? FechaEnvio,
    string? MotivoFallo,
    int Intentos);

public record EnviarBoletinesResultadoDto(
    long CursoId,
    int Total,
    int Exitosos,
    int Fallidos,
    List<EnvioBoletinDto> Detalles);
