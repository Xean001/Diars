-- =========================================
-- ASIGNAR NIVELES A ESTUDIANTES EXISTENTES
-- RunaTalento - Sistema de Gamificación
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? PASO 1: Verificar configuración de niveles';
PRINT '================================================';

-- Verificar que existan niveles configurados
IF NOT EXISTS (SELECT 1 FROM Nivel)
BEGIN
    PRINT '? ERROR: No hay niveles configurados en la tabla Nivel';
    PRINT 'Por favor ejecuta primero: Scripts/ConfigurarNiveles.sql';
    RETURN;
END

SELECT 
    IdNivel,
    Nombre,
    PuntajeMinimo,
    PuntajeMaximo,
    (PuntajeMaximo - PuntajeMinimo + 1) AS RangoPuntos
FROM Nivel
ORDER BY PuntajeMinimo;

PRINT '';
PRINT '?? PASO 2: Verificar estudiantes sin nivel asignado';
PRINT '================================================';

SELECT 
    U.Id,
    U.Nombres + ' ' + U.Apellidos AS Estudiante,
    U.Email,
    U.PuntajeTotal,
    U.IdNivel,
    CASE 
        WHEN U.IdNivel IS NULL THEN '? SIN NIVEL'
        ELSE (SELECT Nombre FROM Nivel WHERE IdNivel = U.IdNivel)
    END AS NivelActual
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
WHERE R.Name = 'Estudiante'
ORDER BY U.PuntajeTotal DESC;

PRINT '';
PRINT '?? PASO 3: Asignar niveles automáticamente';
PRINT '================================================';

-- Actualizar niveles según el puntaje de cada estudiante
UPDATE U
SET U.IdNivel = N.IdNivel
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
INNER JOIN Nivel N ON U.PuntajeTotal >= N.PuntajeMinimo 
                   AND U.PuntajeTotal <= N.PuntajeMaximo
WHERE R.Name = 'Estudiante';

PRINT '? Niveles asignados automáticamente según puntaje';
PRINT '';

PRINT '? PASO 4: Verificar resultado final';
PRINT '================================================';

SELECT 
    U.Nombres + ' ' + U.Apellidos AS Estudiante,
    U.Email,
    U.PuntajeTotal,
    N.Nombre AS Nivel,
    CONCAT(N.PuntajeMinimo, ' - ', N.PuntajeMaximo, ' pts') AS RangoNivel,
    CASE 
        WHEN U.PuntajeTotal >= 1500 THEN '?? Diamante'
        WHEN U.PuntajeTotal >= 900 THEN '?? Platino'
        WHEN U.PuntajeTotal >= 500 THEN '?? Oro'
        WHEN U.PuntajeTotal >= 200 THEN '? Plata'
        ELSE '?? Bronce'
    END AS Icono
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
LEFT JOIN Nivel N ON U.IdNivel = N.IdNivel
WHERE R.Name = 'Estudiante'
ORDER BY U.PuntajeTotal DESC;

PRINT '';
PRINT '?? PASO 5: Estadísticas por nivel';
PRINT '================================================';

SELECT 
    N.Nombre AS Nivel,
    COUNT(U.Id) AS CantidadEstudiantes,
    MIN(U.PuntajeTotal) AS PuntajeMinimo,
    MAX(U.PuntajeTotal) AS PuntajeMaximo,
    AVG(U.PuntajeTotal) AS PuntajePromedio,
    CONCAT(
        CAST(COUNT(U.Id) * 100.0 / NULLIF((
            SELECT COUNT(*) 
            FROM AspNetUsers UU
            INNER JOIN AspNetUserRoles URR ON UU.Id = URR.UserId
            INNER JOIN AspNetRoles RR ON URR.RoleId = RR.Id
            WHERE RR.Name = 'Estudiante'
        ), 0) AS DECIMAL(5,2)),
        '%'
    ) AS Porcentaje
FROM Nivel N
LEFT JOIN AspNetUsers U ON N.IdNivel = U.IdNivel
LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
LEFT JOIN AspNetRoles R ON UR.RoleId = R.Id AND R.Name = 'Estudiante'
GROUP BY N.Nombre, N.PuntajeMinimo
ORDER BY N.PuntajeMinimo;

PRINT '';
PRINT '?? RESUMEN FINAL';
PRINT '================================================';

DECLARE @TotalEstudiantes INT;
DECLARE @EstudiantesConNivel INT;
DECLARE @EstudiantesSinNivel INT;

SELECT @TotalEstudiantes = COUNT(*)
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
WHERE R.Name = 'Estudiante';

SELECT @EstudiantesConNivel = COUNT(*)
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
WHERE R.Name = 'Estudiante' AND U.IdNivel IS NOT NULL;

SET @EstudiantesSinNivel = @TotalEstudiantes - @EstudiantesConNivel;

PRINT '?? Total de estudiantes: ' + CAST(@TotalEstudiantes AS VARCHAR);
PRINT '? Estudiantes con nivel: ' + CAST(@EstudiantesConNivel AS VARCHAR);
PRINT '? Estudiantes sin nivel: ' + CAST(@EstudiantesSinNivel AS VARCHAR);

IF @EstudiantesSinNivel = 0
BEGIN
    PRINT '';
    PRINT '?? ¡PERFECTO! Todos los estudiantes tienen nivel asignado';
END
ELSE
BEGIN
    PRINT '';
    PRINT '?? ATENCIÓN: Hay estudiantes sin nivel';
    PRINT 'Esto es normal si:';
    PRINT '  • Son estudiantes nuevos que aún no han sido calificados';
    PRINT '  • Tienen PuntajeTotal = 0';
    PRINT '';
    PRINT 'Estudiantes sin nivel:';
    
    SELECT 
        U.Nombres + ' ' + U.Apellidos AS Estudiante,
        U.Email,
        U.PuntajeTotal,
        'Completará actividades para obtener nivel' AS Estado
    FROM AspNetUsers U
    INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
    INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
    WHERE R.Name = 'Estudiante' AND U.IdNivel IS NULL
    ORDER BY U.PuntajeTotal DESC;
END

PRINT '';
PRINT '? PROCESO COMPLETADO';
PRINT '================================================';
PRINT 'Los niveles se actualizarán automáticamente cuando:';
PRINT '  • Los estudiantes completen actividades';
PRINT '  • Los docentes califiquen esas actividades';
PRINT '  • El sistema asignará el nivel según el puntaje acumulado';
