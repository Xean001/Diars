-- =========================================
-- SCRIPT RÁPIDO: Asignar Niveles a Estudiantes
-- RunaTalento - Ejecutar AHORA
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? Asignando niveles a estudiantes...';

-- Asignar niveles automáticamente según puntaje
UPDATE U
SET U.IdNivel = N.IdNivel
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
INNER JOIN Nivel N ON U.PuntajeTotal >= N.PuntajeMinimo 
                   AND U.PuntajeTotal <= N.PuntajeMaximo
WHERE R.Name = 'Estudiante';

PRINT '? Niveles asignados';

-- Verificar resultado
SELECT 
    U.Nombres + ' ' + U.Apellidos AS Estudiante,
    U.PuntajeTotal AS Puntos,
    N.Nombre AS Nivel
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
LEFT JOIN Nivel N ON U.IdNivel = N.IdNivel
WHERE R.Name = 'Estudiante'
ORDER BY U.PuntajeTotal DESC;

PRINT '? LISTO - Puedes iniciar la aplicación';
