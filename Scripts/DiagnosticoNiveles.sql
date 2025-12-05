-- =========================================
-- VERIFICAR Y CORREGIR NIVELES
-- RunaTalento - Diagnóstico Completo
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? PASO 1: Ver niveles configurados actualmente';
PRINT '================================================';

SELECT 
    IdNivel,
    Nombre,
    PuntajeMinimo,
    PuntajeMaximo
FROM Nivel
ORDER BY PuntajeMinimo;

PRINT '';
PRINT '?? PASO 2: Ver estudiantes y sus puntos';
PRINT '================================================';

SELECT 
    U.Nombres + ' ' + U.Apellidos AS Estudiante,
    U.PuntajeTotal,
    U.IdNivel AS NivelActualID,
    N.Nombre AS NivelActual,
    -- Calcular qué nivel DEBERÍA tener según los rangos
    (
        SELECT TOP 1 Nombre 
        FROM Nivel 
        WHERE U.PuntajeTotal >= PuntajeMinimo 
        AND U.PuntajeTotal <= PuntajeMaximo
    ) AS NivelCorrecto
FROM AspNetUsers U
INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId
INNER JOIN AspNetRoles R ON UR.RoleId = R.Id
LEFT JOIN Nivel N ON U.IdNivel = N.IdNivel
WHERE R.Name = 'Estudiante'
ORDER BY U.PuntajeTotal DESC;

PRINT '';
PRINT '?? PROBLEMA DETECTADO:';
PRINT 'Los rangos de puntos de los niveles NO cubren todos los puntajes de los estudiantes';
PRINT '';
PRINT '?? SOLUCIONES:';
PRINT '1. Opción A: Ajustar los rangos de niveles en el Admin';
PRINT '2. Opción B: Eliminar niveles actuales y usar la configuración estándar';
PRINT '';
PRINT '¿Qué prefieres hacer?';
