-- =========================================
-- CONFIGURAR NIVELES PRIMERO
-- RunaTalento - EJECUTAR ANTES DE ASIGNAR
-- =========================================

USE RunaTalentoDB;
GO

-- Verificar si ya existen niveles
IF EXISTS (SELECT 1 FROM Nivel)
BEGIN
    PRINT '?? Ya existen niveles configurados';
    SELECT * FROM Nivel ORDER BY PuntajeMinimo;
    RETURN;
END

PRINT '?? Configurando niveles del sistema...';

-- Insertar los 5 niveles estándar
INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Bronce', 0, 199),
('Plata', 200, 499),
('Oro', 500, 899),
('Platino', 900, 1499),
('Diamante', 1500, 999999);

PRINT '? Niveles creados exitosamente';
PRINT '';

-- Mostrar niveles configurados
SELECT 
    IdNivel,
    Nombre,
    PuntajeMinimo,
    PuntajeMaximo,
    CONCAT(PuntajeMinimo, ' - ', PuntajeMaximo, ' pts') AS Rango
FROM Nivel
ORDER BY PuntajeMinimo;

PRINT '';
PRINT '?? Ahora ejecuta: Scripts\AsignarNivelesRapido.sql';
