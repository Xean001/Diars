-- =========================================
-- ACTUALIZAR MEDALLAS EXISTENTES CON IDDOCENTE
-- RunaTalento - Asignar docente a medallas antiguas
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? Verificando medallas sin docente asignado...';
PRINT '';

SELECT 
    ME.IdMedallaEstudiante,
    M.Nombre AS Medalla,
    E.Nombres + ' ' + E.Apellidos AS Estudiante,
    ME.FechaOtorgada,
    ME.IdDocente
FROM MedallaEstudiante ME
INNER JOIN Medalla M ON ME.IdMedalla = M.IdMedalla
INNER JOIN AspNetUsers E ON ME.IdEstudiante = E.Id
WHERE ME.IdDocente IS NULL;

PRINT '';
PRINT '?? Si hay medallas sin docente, puedes asignar un docente por defecto:';
PRINT '';

/*
-- DESCOMENTAR Y MODIFICAR CON EL ID DEL DOCENTE CORRECTO:

-- Opción 1: Asignar todas las medallas sin docente a un docente específico
UPDATE MedallaEstudiante
SET IdDocente = 'ID_DEL_DOCENTE_AQUI'
WHERE IdDocente IS NULL;

-- Opción 2: Ver lista de docentes disponibles
SELECT Id, Nombres, Apellidos, Email
FROM AspNetUsers
WHERE Id IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Docente'));
*/

PRINT '';
PRINT '? Verificación completada';
PRINT 'Las nuevas medallas se asignarán automáticamente al docente que las otorgue';
