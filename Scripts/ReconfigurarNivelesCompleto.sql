-- =========================================
-- SOLUCIÓN: Reconfigurar Niveles Correctamente
-- RunaTalento - Para cubrir todos los puntajes
-- =========================================

USE RunaTalentoDB;
GO

PRINT '??? PASO 1: Eliminar niveles incorrectos';
PRINT '================================================';

-- Primero, quitar niveles de los usuarios
UPDATE AspNetUsers
SET IdNivel = NULL
WHERE IdNivel IS NOT NULL;

-- Luego, eliminar los niveles
DELETE FROM Nivel;

-- Reiniciar el identity
DBCC CHECKIDENT ('Nivel', RESEED, 0);

PRINT '? Niveles antiguos eliminados';
PRINT '';

PRINT '?? PASO 2: Crear niveles que cubran TODOS los rangos';
PRINT '================================================';

INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Bronce', 0, 99),
('Plata', 100, 299),
('Oro', 300, 499),
('Platino', 500, 799),
('Diamante', 800, 999999);

PRINT '? Niveles creados:';
SELECT IdNivel, Nombre, PuntajeMinimo, PuntajeMaximo FROM Nivel ORDER BY PuntajeMinimo;
PRINT '';

PRINT '?? PASO 3: Asignar niveles a estudiantes';
PRINT '================================================';

UPDATE U
SET U.IdNivel = N.IdNivel
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
INNER JOIN Nivel N ON U.PuntajeTotal >= N.PuntajeMinimo 
                   AND U.PuntajeTotal <= N.PuntajeMaximo
WHERE R.Name = 'Estudiante';

PRINT '? Niveles asignados';
PRINT '';

PRINT '?? PASO 4: Verificar resultado';
PRINT '================================================';

SELECT 
    U.Nombres + ' ' + U.Apellidos AS Estudiante,
    U.PuntajeTotal AS Puntos,
    N.Nombre AS Nivel,
    CASE 
        WHEN U.PuntajeTotal >= 800 THEN '?? Diamante'
        WHEN U.PuntajeTotal >= 500 THEN '?? Platino'
        WHEN U.PuntajeTotal >= 300 THEN '?? Oro'
        WHEN U.PuntajeTotal >= 100 THEN '? Plata'
        ELSE '?? Bronce'
    END AS Icono
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
LEFT JOIN Nivel N ON U.IdNivel = N.IdNivel
WHERE R.Name = 'Estudiante'
ORDER BY U.PuntajeTotal DESC;

PRINT '';
PRINT '? COMPLETADO - Ahora todos los estudiantes tienen nivel asignado';
PRINT '?? Resultado esperado:';
PRINT '   • 315 pts ? Oro ??';
PRINT '   • 61 pts ? Bronce ??';
