-- ====================================================================
-- Script: InsertPrimerDirector.sql
-- Descripción: Crea el primer Director del sistema, ya que la creación
-- de usuarios desde la API requiere autenticación de un Director existente.
--
-- IMPORTANTE: Antes de ejecutar:
--   1. Aplique las migraciones EF Core (vea README).
--   2. Genere un hash BCrypt para la contraseña deseada usando el script
--      scripts/GenerarHashDirector.csx, o cualquier librería BCrypt en línea.
--   3. Reemplace los valores entre angulares <...> antes de ejecutar.
-- ====================================================================

INSERT INTO directores (
    t_nombre_completo,
    t_cedula,
    t_correo,
    t_password_hash,
    b_activo,
    fecha_reg
) VALUES (
    'Director Inicial',
    '<CEDULA_DEL_DIRECTOR>',
    '<CORREO_DEL_DIRECTOR>',
    '<HASH_BCRYPT_GENERADO>',
    TRUE,
    CURRENT_TIMESTAMP
);

-- Verificación
SELECT n_id, t_nombre_completo, t_cedula, t_correo, b_activo
FROM directores
WHERE t_cedula = '<CEDULA_DEL_DIRECTOR>';
