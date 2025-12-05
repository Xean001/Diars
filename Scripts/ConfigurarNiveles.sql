-- =========================================
-- SCRIPT DE CONFIGURACIÓN DE NIVELES
-- RunaTalento - Sistema de Gamificación
-- =========================================

-- IMPORTANTE: Este script inserta la configuración recomendada de niveles
-- Ejecutar SOLO UNA VEZ durante la configuración inicial

USE RunaTalentoDB;
GO

-- =========================================
-- OPCIÓN 1: CONFIGURACIÓN BÁSICA (3 NIVELES)
-- Ideal para: Cursos cortos, proyectos pequeños
-- =========================================

/*
INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Principiante', 0, 199),
('Intermedio', 200, 499),
('Experto', 500, 999999);
*/

-- =========================================
-- OPCIÓN 2: CONFIGURACIÓN ESTÁNDAR (5 NIVELES) ? RECOMENDADA
-- Ideal para: Mayoría de casos, cursos semestrales
-- =========================================

-- Eliminar niveles existentes si es necesario (DESCOMENTAR SOLO SI ES NECESARIO)
-- DELETE FROM Nivel;
-- DBCC CHECKIDENT ('Nivel', RESEED, 0);

INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Bronce', 0, 199),
('Plata', 200, 499),
('Oro', 500, 899),
('Platino', 900, 1499),
('Diamante', 1500, 999999);

-- =========================================
-- OPCIÓN 3: CONFIGURACIÓN AVANZADA (7 NIVELES)
-- Ideal para: Programas largos, múltiples cursos
-- =========================================

/*
INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Novato', 0, 99),
('Aprendiz', 100, 249),
('Competente', 250, 499),
('Hábil', 500, 799),
('Experto', 800, 1199),
('Maestro', 1200, 1799),
('Leyenda', 1800, 999999);
*/

-- =========================================
-- OPCIÓN 4: CONFIGURACIÓN TEMÁTICA - ESPACIAL ??
-- Ideal para: Temas de tecnología, ciencia
-- =========================================

/*
INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Cadet', 0, 149),
('Pilot', 150, 349),
('Captain', 350, 649),
('Commander', 650, 999),
('Admiral', 1000, 999999);
*/

-- =========================================
-- OPCIÓN 5: CONFIGURACIÓN EDUCATIVA
-- Ideal para: Sistema escolar tradicional
-- =========================================

/*
INSERT INTO Nivel (Nombre, PuntajeMinimo, PuntajeMaximo) VALUES
('Freshman', 0, 199),
('Sophomore', 200, 399),
('Junior', 400, 699),
('Senior', 700, 1099),
('Graduate', 1100, 999999);
*/

-- =========================================
-- VERIFICAR NIVELES INSERTADOS
-- =========================================

SELECT 
    IdNivel,
    Nombre,
    PuntajeMinimo,
    PuntajeMaximo,
    (PuntajeMaximo - PuntajeMinimo + 1) AS RangoPuntos
FROM Nivel
ORDER BY PuntajeMinimo;

-- =========================================
-- ESTADÍSTICAS DE DISTRIBUCIÓN DE ESTUDIANTES
-- =========================================

SELECT 
    N.Nombre AS Nivel,
    COUNT(U.Id) AS CantidadEstudiantes,
    CAST(COUNT(U.Id) * 100.0 / NULLIF((SELECT COUNT(*) FROM AspNetUsers WHERE IdNivel IS NOT NULL), 0) AS DECIMAL(5,2)) AS Porcentaje
FROM Nivel N
LEFT JOIN AspNetUsers U ON N.IdNivel = U.IdNivel
GROUP BY N.Nombre, N.PuntajeMinimo
ORDER BY N.PuntajeMinimo;

-- =========================================
-- ESTUDIANTES SIN NIVEL ASIGNADO
-- =========================================

SELECT 
    Id,
    Nombres,
    Apellidos,
    Email,
    PuntajeTotal,
    'Sin nivel asignado' AS Observacion
FROM AspNetUsers
WHERE IdNivel IS NULL
ORDER BY PuntajeTotal DESC;

-- =========================================
-- ASIGNAR NIVELES A ESTUDIANTES EXISTENTES
-- (Ejecutar si ya tienes estudiantes en el sistema)
-- =========================================

/*
UPDATE U
SET U.IdNivel = N.IdNivel
FROM AspNetUsers U
INNER JOIN Nivel N ON U.PuntajeTotal >= N.PuntajeMinimo 
                   AND U.PuntajeTotal <= N.PuntajeMaximo
WHERE U.IdNivel IS NULL;
*/

-- =========================================
-- CONSULTA DE VALIDACIÓN FINAL
-- =========================================

-- Verificar que no hay solapamientos
SELECT 
    N1.Nombre AS Nivel1,
    N1.PuntajeMinimo AS Min1,
    N1.PuntajeMaximo AS Max1,
    N2.Nombre AS Nivel2,
    N2.PuntajeMinimo AS Min2,
    N2.PuntajeMaximo AS Max2,
    'SOLAPAMIENTO DETECTADO' AS Alerta
FROM Nivel N1
CROSS JOIN Nivel N2
WHERE N1.IdNivel < N2.IdNivel
  AND (
      (N1.PuntajeMinimo BETWEEN N2.PuntajeMinimo AND N2.PuntajeMaximo) OR
      (N1.PuntajeMaximo BETWEEN N2.PuntajeMinimo AND N2.PuntajeMaximo) OR
      (N1.PuntajeMinimo <= N2.PuntajeMinimo AND N1.PuntajeMaximo >= N2.PuntajeMaximo)
  );

-- Si esta consulta no devuelve filas, ¡perfecto! No hay solapamientos

-- =========================================
-- FIN DEL SCRIPT
-- =========================================

PRINT '? Configuración de niveles completada';
PRINT 'Verifica los resultados de las consultas anteriores';
PRINT 'Si todo está correcto, el sistema está listo para usar';
