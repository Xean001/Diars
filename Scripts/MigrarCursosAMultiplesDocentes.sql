-- =========================================
-- MIGRAR CURSOS EXISTENTES A MÚLTIPLES DOCENTES
-- RunaTalento - Sistema de Gamificación
-- =========================================

USE RunaTalentoDB;
GO

PRINT '?? PASO 1: Verificar cursos existentes';
PRINT '================================================';

SELECT 
    IdCurso,
    Nombre,
    IdDocente,
    (SELECT Nombres + ' ' + Apellidos FROM AspNetUsers WHERE Id = Curso.IdDocente) AS DocenteActual
FROM Curso
WHERE IdDocente IS NOT NULL;

PRINT '';
PRINT '?? PASO 2: Migrar docentes a tabla CursoDocente';
PRINT '================================================';

-- Insertar docentes existentes en la nueva tabla intermedia
INSERT INTO CursoDocente (IdCurso, IdDocente, FechaAsignacion)
SELECT 
    IdCurso,
    IdDocente,
    GETDATE()
FROM Curso
WHERE IdDocente IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM CursoDocente 
    WHERE CursoDocente.IdCurso = Curso.IdCurso 
    AND CursoDocente.IdDocente = Curso.IdDocente
);

PRINT '? Docentes migrados a la tabla CursoDocente';
PRINT '';

PRINT '?? PASO 3: Verificar migración';
PRINT '================================================';

SELECT 
    C.Nombre AS Curso,
    U.Nombres + ' ' + U.Apellidos AS Docente,
    CD.FechaAsignacion
FROM CursoDocente CD
INNER JOIN Curso C ON CD.IdCurso = C.IdCurso
INNER JOIN AspNetUsers U ON CD.IdDocente = U.Id
ORDER BY C.Nombre, U.Apellidos;

PRINT '';
PRINT '? MIGRACIÓN COMPLETADA';
PRINT 'Ahora puedes asignar múltiples docentes a cada curso';
