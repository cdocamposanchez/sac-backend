# Sistema Académico Cayzedo (SAC)

API REST/JSON del Sistema Académico Cayzedo, plataforma digital para los docentes y el director de la **Institución Educativa Joaquín de Cayzedo y Cuero**. Sustituye el flujo basado en Excel y cuadernos por una solución centralizada con login, gestión de cursos, cálculo automático de promedios, reportes finales y envío de boletines por correo.

> Construido en **.NET 8** con arquitectura **Clean + DDD + CQRS** (mismo patrón que el proyecto de referencia *oasis-fur*).

---

## ¿Qué hace SAC?

Es una plataforma **web** que se accede desde el navegador en computadores o tablets del colegio. Permite a Directores y Docentes gestionar las calificaciones académicas de los estudiantes en tiempo real.

### Roles

| Rol | Puede |
|-----|-------|
| **Director** | Gestionar Docentes, Estudiantes, Grados, Materias, Cursos, asignaciones, periodos. Cerrar periodos y cursos. Enviar boletines. Consultar todos los cursos. |
| **Docente** | Crear actividades, registrar/editar/eliminar calificaciones (solo en sus cursos y solo si el periodo está abierto). Consultar sus cursos. |
| **Estudiante** | **No accede al sistema**; solo recibe boletines por correo. |

### Casos de uso (HU-01 a HU-11)

1. **HU-01 / RF-01 — Login** por cédula y contraseña; redirección por rol vía JWT.
2. **HU-02 / RF-02 — Gestión de Docentes** (CRUD, lote no aplica para docentes).
3. **HU-03 / RF-02 — Gestión de Estudiantes** (CRUD + carga en lote).
4. **HU-04 / RF-03 — Catálogo de Grados** (6-1, 6-2, 7-1...).
5. **HU-05 / RF-03 — Catálogo de Materias** (Matemáticas, Español, Ciencias...).
6. **HU-06 / RF-04 — Cursos**: combinación Grado + Materia + Docente. Al crearse, el sistema genera automáticamente los 3 periodos (Corte 1, 2 y 3).
7. **HU-07 / RF-05 — Asignación de estudiantes** a cursos (individual o en lote).
8. **HU-08 / RF-07 — Actividades** dentro de un curso/periodo abierto (solo el docente del curso).
9. **HU-09 / RF-08, RF-09 — Calificaciones** rango 1.0–5.0, con suma de porcentajes ≤ 100% por curso/actividad/periodo. Recálculo automático de promedios.
10. **HU-10 / RF-04, RF-06 — Cierre de periodos y cursos**. Un curso solo se cierra cuando los 3 periodos están cerrados.
11. **HU-11 / RF-11 — Boletines** individuales por correo a cada estudiante (solo cuando el curso está cerrado).

---

## Arquitectura

```
sac/
├── BuildingBlocks/
│   └── BuildingBlocks/
│       └── Source/
│           ├── Application/    (CQRS interfaces, Behaviors, Utils, DTOs comunes)
│           ├── Domain/         (Entity, Aggregate, IDomainEvent, Excepciones)
│           └── Infrastructure/ (Config, JWT, Email, Middleware, Interceptors)
├── Services/
│   └── Sac/
│       ├── Sac.Domain/         (Modelos DDD con lógica de negocio)
│       ├── Sac.Application/    (Commands/Queries con MediatR)
│       ├── Sac.Infrastructure/ (EF Core, persistencia, servicios externos)
│       └── Sac.Api/            (Controllers REST)
├── postman/                     (Colección Postman lista para importar)
├── docs/                        (arquitectura.md y otros documentos)
├── scripts/                     (SQL para crear primer Director)
├── docker-compose.yaml          (Solo MongoDB + API; Postgres se configura aparte)
├── .env.template
└── Sac.sln
```

### Estructura de Modules (CQRS)

Cada módulo se organiza por agregado y separa Commands/Queries; cada acción vive en su propia carpeta con `Command/Query`, `Validator` y `Handler`:

```
Modules/
└── Calificaciones/
    ├── Commands/
    │   ├── CreateCalificacion/
    │   │   ├── CreateCalificacionCommand.cs
    │   │   ├── CreateCalificacionValidator.cs
    │   │   └── CreateCalificacionCommandHandler.cs
    │   ├── UpdateCalificacion/
    │   │   ├── UpdateCalificacionCommand.cs
    │   │   ├── UpdateCalificacionValidator.cs
    │   │   └── UpdateCalificacionCommandHandler.cs
    │   └── DeleteCalificacion/
    │       ├── DeleteCalificacionCommand.cs
    │       └── DeleteCalificacionCommandHandler.cs
    ├── Queries/
    │   ├── GetCalificaciones/
    │   │   ├── GetCalificacionesQuery.cs
    │   │   └── GetCalificacionesQueryHandler.cs
    │   └── GetCalificacionById/
    │       ├── GetCalificacionByIdQuery.cs
    │       └── GetCalificacionByIdQueryHandler.cs
    └── _Shared/
        └── CalificacionShared.cs    (helpers internos del módulo)
```

### Capas

- **Domain** — Agregados raíz (`Director`, `Docente`, `Estudiante`, `Grado`, `Materia`, `Curso`, `CursoEstudiante`, `Periodo`, `Actividad`, `Calificacion`, `EnvioBoletin`). Toda la lógica de negocio vive aquí: invariantes en `Calificacion.Create`, `Curso.Cerrar(periodos)`, `Periodo.Cerrar()`, etc. Setters privados, factories `Create()`, eventos de dominio (`CalificacionRegistradaEvent`, `PeriodoCerradoEvent`, `CursoCerradoEvent`).
- **Application** — Casos de uso vía MediatR + FluentValidation. Pipeline behaviors (`ValidationBehavior`, `LoggingBehavior`).
- **Infrastructure** — `ApplicationDbContext` (EF Core + Npgsql), 11 EF Configurations, interceptores para auditoría y dispatch de eventos.
- **Api** — Controllers REST agrupados por recurso; auth JWT con políticas (`DirectorOnly`, `DocenteOnly`, `DirectorOrDocente`); middleware global `ApiResponseMiddleware`; `CustomExceptionHandler`.

---

## Stack

| Componente | Tecnología |
|------------|------------|
| Runtime | .NET 8 |
| ORM | Entity Framework Core 9 + Npgsql |
| BD transaccional | PostgreSQL |
| BD auditoría/logs | MongoDB (en docker-compose) |
| Mediator | MediatR 12 |
| Validación | FluentValidation 12 |
| Mapper | Mapster |
| Auth | JWT Bearer (HMAC-SHA256) |
| Hashing | BCrypt.Net (workFactor 12) |
| Correo | MailKit (SMTP Google) |
| Documentación API | Swagger / OpenAPI 3 |

---

## Requisitos

- .NET SDK 8.0
- Docker y Docker Compose (opcional, para Mongo y la API)
- PostgreSQL 14+ (instalación local, RDS, Cloud SQL u otro)
- Cuenta Gmail con **App Password** para SMTP

---

## Puesta en marcha

### 1. Variables de entorno

```bash
cp .env.template .env
# Editar .env con tus valores (DB, JWT_SECRET_KEY, SMTP, etc.)
```

> ⚠️ **PostgreSQL no está incluido en el `docker-compose.yaml`**. La aplicación lo sigue usando como base de datos transaccional principal; configura tu Postgres aparte (instalación local, RDS, Cloud SQL, etc.) y apunta `DB_HOST`, `DB_PORT`, `DB_USER`, `DB_PASSWORD`, `DB_NAME` a esa instancia.

### 2. Migraciones EF Core

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate \
  -p Services/Sac/Sac.Infrastructure \
  -s Services/Sac/Sac.Api
dotnet ef database update \
  -p Services/Sac/Sac.Infrastructure \
  -s Services/Sac/Sac.Api
```

### 3. Levantar con Docker Compose

```bash
docker compose up -d --build
```

Esto arranca:
- `sac-api` en `http://localhost:8080`
- `sac-mongo` en `localhost:27017`

### 4. Ejecutar en modo desarrollo (sin Docker)

```bash
dotnet run --project Services/Sac/Sac.Api
```

Swagger UI: `http://localhost:8080/api/v1/docs`

### 5. Crear el primer Director

Como la creación de usuarios requiere autenticación de Director, hay que insertar el primero manualmente. Genera el hash con `scripts/GenerarHashDirector.csx` y úsalo en `scripts/InsertPrimerDirector.sql`:

```sql
INSERT INTO directores
  (t_nombre_completo, t_cedula, t_correo, t_password_hash, b_activo, fecha_reg)
VALUES
  ('Director Inicial', '1000000000', 'director@cayzedo.edu',
   '<HASH_BCRYPT_GENERADO>', true, CURRENT_TIMESTAMP);
```

---

## Endpoints principales

Prefijo: **`/api/v1`** · formato: **JSON**

| Recurso | Método | Ruta | Acceso |
|---------|--------|------|--------|
| Auth | POST | `/auth/login` | Público |
| Docentes | POST/PUT/DELETE | `/docentes`, `/docentes/{id}` | Director |
| Docentes | GET | `/docentes`, `/docentes/{id}` | Director / Docente |
| Estudiantes | POST | `/estudiantes`, `/estudiantes/lote` | Director |
| Estudiantes | PUT/DELETE | `/estudiantes/{id}` | Director |
| Estudiantes | GET | `/estudiantes`, `/estudiantes/{id}` | Director / Docente |
| Grados / Materias | POST/PUT/DELETE | `/grados`, `/materias` | Director |
| Grados / Materias | GET | `/grados`, `/materias` | Director / Docente |
| Cursos | POST/PUT | `/cursos`, `/cursos/{id}` | Director |
| Cursos | POST | `/cursos/{id}/cerrar` | Director |
| Cursos | POST | `/cursos/{id}/estudiantes`, `/cursos/{id}/estudiantes/lote` | Director |
| Cursos | DELETE | `/cursos/{id}/estudiantes/{estudianteId}` | Director |
| Cursos | GET | `/cursos`, `/cursos/{id}`, `/cursos/{id}/estudiantes` | Director / Docente |
| Periodos | POST | `/periodos/{id}/cerrar`, `/periodos/{id}/reabrir` | Director |
| Periodos | GET | `/periodos/curso/{cursoId}`, `/periodos/{id}` | Director / Docente |
| Actividades | POST/PUT/DELETE | `/actividades`, `/actividades/{id}` | Docente del curso |
| Actividades | GET | `/actividades`, `/actividades/{id}` | Director / Docente |
| Calificaciones | POST/PUT/DELETE | `/calificaciones`, `/calificaciones/{id}` | Docente del curso |
| Calificaciones | GET | `/calificaciones`, `/calificaciones/{id}` | Director / Docente (filtrado por rol) |
| Reportes | GET | `/reportes/curso/{cursoId}`, `/.../{cursoId}/estudiante/{eId}`, `/.../{cursoId}/final` | Director / Docente |
| Boletines | POST | `/boletines/curso/{cursoId}/enviar`, `/.../reintentar` | Director |
| Boletines | GET | `/boletines/curso/{cursoId}` | Director |
| Health | GET | `/health` | Público |
| Swagger | GET | `/docs` | Público |

Una colección Postman completa con ejemplos está en **`postman/Sac.postman_collection.json`** — el script de Login guarda automáticamente el JWT en la variable `{{token}}`.

---

## Trazabilidad RF / HU → Implementación

| ID | Implementación |
|----|----------------|
| HU-01 / RF-01 | `Modules/Auth/Commands/Login` → JWT con role claim |
| HU-02 / RF-02 | `Modules/Docentes/*`, `Modules/Estudiantes/*` |
| HU-03 / RF-02 | `Modules/Estudiantes/Commands/CreateEstudiante`, `CreateEstudiantesLote` |
| HU-04 / RF-03 | `Modules/Grados/*` |
| HU-05 / RF-03 | `Modules/Materias/*` |
| HU-06 / RF-04 | `Modules/Cursos/Commands/CreateCurso` (auto-crea los 3 periodos) |
| HU-07 / RF-05 | `Modules/CursoEstudiantes/*` |
| HU-08 / RF-07 | `Modules/Actividades/*` (validación de docente titular y periodo abierto) |
| HU-09 / RF-08, RF-09 | `Modules/Calificaciones/*` (rango 1.0-5.0, suma %, recálculo de promedio) |
| HU-10 / RF-04, RF-06 | `Modules/Periodos/Commands/CerrarPeriodo`, `Cursos/Commands/CerrarCurso` |
| HU-11 / RF-11 | `Modules/Boletines/Commands/EnviarBoletines` (curso CERRADO) |

---

## Cumplimiento de RNF

| RNF | Implementación |
|-----|----------------|
| Seguridad | BCrypt(12), JWT HS256, autorización por roles, headers de seguridad |
| Rendimiento | `LoggingBehavior` alerta sobre handlers > 3s, `AsNoTracking` en queries |
| Disponibilidad | Health check `/health`, restart policies en compose |
| Usabilidad | `ProblemDetails` con mensajes claros, validators en español |
| Integridad | Validación dual (FluentValidation + invariantes de dominio), índices únicos |
| Escalabilidad | Modular por agregado, separación clara de capas, MongoDB para auditoría |

---

## Testing manual (Postman)

1. Levantar la API.
2. Importar `postman/Sac.postman_collection.json`.
3. Configurar la variable `baseUrl` (default: `http://localhost:8080/api/v1`).
4. Ejecutar **Auth → Login** con la cédula del primer Director — el script guarda el token automáticamente.
5. Ejecutar el resto en este orden sugerido: catálogos → docentes → estudiantes → cursos → asignaciones → actividades → calificaciones → cerrar periodos → cerrar curso → enviar boletines.
