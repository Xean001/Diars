-- =========================================
-- SCRIPT PARA ARREGLAR NIVELES
-- RunaTalento - Asignar niveles automáticamente
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? PASO 1: Verificar estudiantes sin nivel asignado';
PRINT '================================================';

SELECT 
    Id,
    Nombres,
    Apellidos,
    Email,
    PuntajeTotal,
    IdNivel,
    CASE 
        WHEN IdNivel IS NULL THEN '? Sin nivel'
        ELSE '? Con nivel'
    END AS Estado
FROM AspNetUsers
WHERE Id IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Estudiante'))
ORDER BY PuntajeTotal DESC;

PRINT '';
PRINT '?? PASO 2: Asignar niveles automáticamente según puntaje';
PRINT '================================================';

-- Asignar nivel según puntaje actual
UPDATE U
SET U.IdNivel = N.IdNivel
FROM AspNetUsers U
INNER JOIN Nivel N 
    ON U.PuntajeTotal >= N.PuntajeMinimo 
    AND U.PuntajeTotal <= N.PuntajeMaximo
WHERE U.Id IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Estudiante'));

PRINT '? Niveles asignados exitosamente';
PRINT '';

PRINT '?? PASO 3: Verificar resultado final';
PRINT '================================================';

SELECT 
    U.Nombres,
    U.Apellidos,
    U.PuntajeTotal,
    N.Nombre AS Nivel,
    CONCAT(N.PuntajeMinimo, ' - ', N.PuntajeMaximo, ' pts') AS RangoPuntaje
FROM AspNetUsers U
LEFT JOIN Nivel N ON U.IdNivel = N.IdNivel
WHERE U.Id IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Estudiante'))
ORDER BY U.PuntajeTotal DESC;

PRINT '';
PRINT '? PROCESO COMPLETADO';
