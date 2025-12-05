-- =========================================
-- INSERTAR INCENTIVOS INICIALES
-- RunaTalento - Sistema de Gamificación
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? Insertando incentivos automáticos iniciales...';
PRINT '';

-- Verificar si ya existen incentivos
IF EXISTS (SELECT 1 FROM Incentivo)
BEGIN
    PRINT '?? Ya existen incentivos en la base de datos';
    PRINT 'Si deseas reiniciar, ejecuta:';
    PRINT '  DELETE FROM IncentivoEstudiante;';
    PRINT '  DELETE FROM Incentivo;';
    PRINT '';
    SELECT * FROM Incentivo ORDER BY PuntosRequeridos;
    RETURN;
END

-- Insertar los 3 incentivos mostrados en la imagen
INSERT INTO Incentivo (Nombre, Descripcion, PuntosRequeridos, IconoUrl, Activo, FechaCreacion) VALUES
('Certificado de Excelencia', 'Certificado digital de excelencia académica', 500, 'bi bi-award-fill', 1, GETDATE()),
('Libro Digital', 'Acceso a biblioteca digital premium', 300, 'bi bi-book-fill', 1, GETDATE()),
('Reconocimiento Público', 'Mención en ceremonia virtual', 200, 'bi bi-megaphone-fill', 1, GETDATE());

PRINT '? Incentivos creados exitosamente:';
PRINT '';

SELECT 
    IdIncentivo,
    Nombre,
    Descripcion,
    CONCAT(PuntosRequeridos, ' pts') AS Puntos,
    CASE WHEN Activo = 1 THEN '? Activo' ELSE '?? Inactivo' END AS Estado,
    IconoUrl
FROM Incentivo
ORDER BY PuntosRequeridos;

PRINT '';
PRINT '?? CÓMO FUNCIONA:';
PRINT '================================================';
PRINT '1. Los incentivos se otorgan AUTOMÁTICAMENTE cuando:';
PRINT '   • El docente califica una actividad';
PRINT '   • El estudiante alcanza o supera los puntos requeridos';
PRINT '   • El incentivo está ACTIVO';
PRINT '';
PRINT '2. Para activar/desactivar un incentivo:';
PRINT '   • Ve a Admin ? Incentivos';
PRINT '   • Click en el botón ?? (Pausar) o ?? (Activar)';
PRINT '';
PRINT '3. Los estudiantes verán sus incentivos en su perfil';
PRINT '';
PRINT '? SISTEMA DE INCENTIVOS LISTO';
