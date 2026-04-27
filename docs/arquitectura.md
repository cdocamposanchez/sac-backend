# Documento de Arquitectura — Sistema Académico Cayzedo (SAC)

## 1. Visión

API REST/JSON para gestionar las calificaciones académicas del colegio Joaquín de Cayzedo y Cuero. Reemplaza el flujo actual basado en Excel/cuadernos por una plataforma centralizada con login, gestión de cursos, cálculo automático de promedios, reportes finales y envío de boletines por correo.

## 2. Estilo arquitectónico

**Clean Architecture + Domain-Driven Design** (mismo patrón que el proyecto referente *oasis-fur*).

```
┌─────────────────────────────────────────┐
│              Api (Controllers)          │   <- Capa de presentación
│   AuthController, CalificacionController│
├─────────────────────────────────────────┤
│           Application                   │   <- Casos de uso (CQRS + MediatR)
│   Modules/{Auth, Cursos, Calificaciones}│
├─────────────────────────────────────────┤
│             Domain                      │   <- Modelos, lógica de negocio
│   Aggregates: Curso, Calificacion       │
├─────────────────────────────────────────┤
│           Infrastructure                │   <- EF Core, Email, JWT
│   ApplicationDbContext, SmtpEmailService│
└─────────────────────────────────────────┘
```

Las dependencias siempre apuntan hacia **adentro**: la capa Api referencia Application e Infrastructure; Application referencia Domain; Domain no referencia nada externo.

### Building blocks compartidos

- `BuildingBlocks` provee abstracciones reutilizables: `Aggregate<T>`, `IDomainEvent`, `Entity<T>`, excepciones, `ApiResponseDto`, `ApiResponseMiddleware`, `IJwtTokenService`, `IEmailService`, interceptores EF.

## 3. Módulos / Bounded contexts

| Módulo | Responsabilidad | RFs |
|--------|-----------------|-----|
| Auth | Login JWT | RF-01 |
| Docentes | CRUD docentes | RF-02 |
| Estudiantes | CRUD estudiantes (incl. carga lote) | RF-02 |
| Grados / Materias | Catálogos | RF-03 |
| Cursos | Crear/editar/cerrar cursos, asignar docente | RF-04 |
| CursoEstudiantes | Asignación estudiantes ↔ curso | RF-05 |
| Periodos | Cierre/reapertura de periodos | RF-06 |
| Actividades | CRUD actividades por curso/periodo | RF-07 |
| Calificaciones | Registro de notas con validaciones | RF-08, RF-09 |
| Reportes | Reportes por estudiante/curso/final | RF-10 |
| Boletines | Envío individual por correo + reintentos | RF-11 |

## 4. Decisiones clave

### 4.1 Modelos DDD con lógica de negocio

Cada agregado es responsable de mantener sus invariantes. Ejemplos:

- `Calificacion.Create()` valida que la nota esté en `[1.0, 5.0]` y porcentaje en `[0, 100]`.
- `Calificacion.Actualizar(nota, porcentaje, periodoAbierto)` exige que el periodo esté abierto.
- `Calificacion.ValidarSumaPorcentajes(suma, nuevo)` impide pasar de 100% en cada curso/actividad/periodo (RF-08).
- `Curso.Cerrar(periodos)` exige los **3** periodos cerrados.
- `Periodo.Cerrar()` emite el evento `PeriodoCerradoEvent` automáticamente.

Setters privados, constructores privados, `Aggregate<T>.AddDomainEvent()` para eventos.

### 4.2 CQRS con MediatR

Cada caso de uso es un Command/Query independiente, con su Validator y Handler. Pipeline behaviors:
- `ValidationBehavior` ejecuta los validators FluentValidation antes del handler.
- `LoggingBehavior` registra latencia y advierte sobre handlers > 3s.

### 4.3 Recálculo automático de promedios (RF-09)

Cada vez que se crea, edita o elimina una calificación:
1. Se valida la suma de porcentajes (suma actual + nuevo ≤ 100).
2. Se persiste el cambio.
3. Se recalcula el promedio del periodo (`PromedioCalculator.PromedioPeriodo`).
4. Se persiste el promedio actualizado.

Todo en una transacción para garantizar atomicidad.

El promedio final del curso se calcula en el reporte cuando los 3 periodos están cerrados.

### 4.4 Autenticación y autorización

- JWT firmado HS256 con clave del entorno (`JWT_SECRET_KEY`).
- Token con claims `sub`, `cedula`, `nombre`, `rol`.
- Tres políticas: `DirectorOnly`, `DocenteOnly`, `DirectorOrDocente`.
- Filtrado por rol en queries: un Docente solo ve cursos donde es titular.

### 4.5 Persistencia

- **PostgreSQL** para datos transaccionales: 11 tablas con índices únicos críticos:
  - `(estudiante, actividad)` evita doble nota.
  - `(curso, periodo, numero_corte)` evita periodos duplicados.
  - `(grado, materia, anio_academico)` evita cursos duplicados.
- **MongoDB** disponible vía docker-compose para auditoría/logs futuros (no se usa actualmente en código).
- Auditoría automática `FechaReg`/`FechaMod` con `AuditableEntityInterceptor`.

### 4.6 Sin docker-compose para Postgres (decisión del cliente)

El compose solo levanta Mongo + API. Postgres se debe configurar aparte (instalación local, RDS, Cloud SQL, otro compose, etc.). Las variables `DB_HOST`, `DB_PORT`, `DB_USER`, `DB_PASSWORD`, `DB_NAME` apuntan a esa instancia.

### 4.7 Email

- Servicio `SmtpEmailService` usa MailKit con StartTLS.
- Configurado para SMTP de Gmail por defecto.
- Cuerpo del boletín en HTML inline con el detalle por periodo y promedio final.
- Cada envío se persiste en `envios_boletin` con estado y reintentos.

## 5. Flujos críticos

### 5.1 Flujo de calificación

```
Docente → POST /calificaciones → CreateCalificacionCommand
  → Validator: nota 1-5, porcentaje 0-100
  → Handler:
     1. Validar curso/periodo/actividad existen
     2. Validar permiso (curso del docente)
     3. Validar periodo abierto
     4. Validar estudiante inscrito en curso
     5. Validar suma porcentajes ≤ 100
     6. Calificacion.Create() (invariantes en agregado)
     7. SaveChanges
     8. RecalcularPromedioPeriodo
     9. SaveChanges
     10. Commit transacción
  → Response 200 + ApiResponseDto wrap
```

### 5.2 Flujo de cierre y boletines

```
Director cierra los 3 periodos →
Director cierra el curso (Curso.Cerrar(periodos)) →
   - Valida los 3 cerrados
   - Estado = Cerrado
   - Emite CursoCerradoEvent
Director envía boletines (POST /boletines/curso/{id}/enviar) →
   - Para cada estudiante:
     * Construye reporte
     * Construye HTML
     * SmtpEmailService.SendAsync
     * Persiste resultado en envios_boletin
   - Retorna resumen (exitosos / fallidos)
```

## 6. RNF

| RNF | Implementación |
|-----|----------------|
| RNF-01 Seguridad | BCrypt(12), JWT, políticas de autorización, headers de seguridad |
| RNF-02 Performance | Async/await, AsNoTracking en queries, LoggingBehavior alerta >3s |
| RNF-03 Disponibilidad | Health check /api/v1/health, docker-compose con restart policies |
| RNF-04 Usabilidad | Mensajes de error claros (CustomExceptionHandler con problemDetails) |
| RNF-05 Integridad | Validación dual (FluentValidation + invariantes de dominio) + índices únicos |
| RNF-06 Mantenibilidad | Clean Architecture, DDD, módulos cohesivos, docs |
