-- Script para agregar la columna NivelDificultad a la tabla Actividad

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Actividad' 
    AND COLUMN_NAME = 'NivelDificultad'
)
BEGIN
    -- Agregar la columna NivelDificultad
    ALTER TABLE Actividad
    ADD NivelDificultad NVARCHAR(20) NULL;
    
    -- Establecer valor por defecto "normal" para registros existentes
    UPDATE Actividad
    SET NivelDificultad = 'normal'
    WHERE NivelDificultad IS NULL;
    
    PRINT 'Columna NivelDificultad agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna NivelDificultad ya existe';
END
GO

-- Actualizar el historial de migraciones si es necesario
IF NOT EXISTS (
    SELECT * FROM __EFMigrationsHistory 
    WHERE MigrationId = '20251128021435_AgregarNivelDificultadActividad'
)
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251128021435_AgregarNivelDificultadActividad', '8.0.0');
    
    PRINT 'Migración registrada en historial';
END
ELSE
BEGIN
    PRINT 'Migración ya está registrada';
END
GO
