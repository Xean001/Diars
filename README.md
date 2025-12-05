# ?? RunaTalento - Sistema de Gamificación Educativa

Sistema de gestión educativa con gamificación para mejorar la motivación y el rendimiento académico de los estudiantes.

## ?? Características

- ? **Gestión de Usuarios**: Admin, Docentes y Estudiantes
- ? **Sistema de Actividades**: Creación y entrega de tareas
- ? **Gamificación**: Puntos, niveles, medallas e incentivos
- ? **Calificación Avanzada**: Múltiples estrategias de calificación
- ? **Rankings**: Competencia saludable entre estudiantes
- ? **Patrones de Diseño**: GoF (Strategy, Observer, Decorator, Factory) y GRASP

## ??? Arquitectura

### Tecnologías
- **Framework**: ASP.NET Core 8.0 MVC + Razor Pages
- **Base de Datos**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Autenticación**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Bootstrap Icons
- **Despliegue**: Docker + Coolify

### Patrones Implementados

#### Patrones GoF
- **Strategy**: Diferentes estrategias de calificación
- **Observer**: Notificaciones de eventos de calificación
- **Decorator**: Decoradores para actividades (dificultad, urgencia)
- **Factory Method**: Creación de estrategias de calificación

#### Patrones GRASP
- **Controller**: Lógica de negocio coordinada
- **High Cohesion**: Servicios especializados
- **Information Expert**: Responsabilidades en modelos
- **Low Coupling**: Inyección de dependencias

### Estructura del Proyecto

```
RunaTalento/
??? Controllers/          # Controladores MVC (Capa de Presentación)
??? BusinessLogic/        # Lógica de Negocio
??? Services/            # Servicios de Aplicación
?   ??? Strategies/      # Patrón Strategy
?   ??? Observers/       # Patrón Observer
?   ??? Decorators/      # Patrón Decorator
?   ??? Factories/       # Patrón Factory Method
??? Models/              # Modelos de Dominio
??? Data/                # Contexto de Base de Datos
??? Views/               # Vistas Razor
??? wwwroot/             # Archivos estáticos
```

## ?? Inicio Rápido

### Pre-requisitos

- .NET 8.0 SDK
- SQL Server (base de datos ya desplegada)
- Docker (opcional, para despliegue)

### Instalación Local

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/RunaTalento.git
   cd RunaTalento
   ```

2. **Configurar la cadena de conexión**
   
   Editar `RunaTalento/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=RunaTalentoDB;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
     }
   }
   ```

3. **Restaurar paquetes**
   ```bash
   cd RunaTalento
   dotnet restore
   ```

4. **Aplicar migraciones**
   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a la aplicación**
   ```
   https://localhost:7224
   ```

### Usuario Administrador por Defecto

- **Email**: `admin@runatalento.com`
- **Contraseña**: `Admin123!`

## ?? Despliegue con Docker

### Prueba Local

```powershell
# Construir y ejecutar
.\deploy.ps1 -Action run

# Probar la aplicación
.\deploy.ps1 -Action test

# Ver logs
.\deploy.ps1 -Action logs

# Detener
.\deploy.ps1 -Action stop
```

### Despliegue en Coolify

Ver guía completa en [DEPLOYMENT.md](DEPLOYMENT.md)

**Pasos resumidos:**
1. Conectar repositorio Git en Coolify
2. Seleccionar Dockerfile como Build Pack
3. Configurar puerto 8080
4. Agregar variables de entorno
5. Deploy

## ?? Endpoints Principales

| Ruta | Descripción | Rol |
|------|-------------|-----|
| `/` | Página principal | Público |
| `/Identity/Account/Login` | Inicio de sesión | Público |
| `/Identity/Account/Register` | Registro | Público |
| `/Home/Index` | Dashboard | Autenticado |
| `/Usuarios` | Gestión de usuarios | Admin |
| `/Medalla` | Gestión de medallas | Admin |
| `/Incentivo` | Gestión de incentivos | Admin |
| `/Docente` | Panel de docente | Docente |
| `/ActividadesEstudiante` | Actividades del estudiante | Estudiante |
| `/Ranking` | Rankings | Estudiante |
| `/health` | Health check | Sistema |

## ?? Roles y Permisos

### Admin
- Gestión completa de usuarios
- Configuración de medallas e incentivos
- Gestión de niveles
- Acceso a todas las funcionalidades

### Docente
- Crear y gestionar actividades
- Calificar entregas de estudiantes
- Otorgar medallas
- Ver rankings de sus cursos

### Estudiante
- Ver y realizar actividades
- Subir entregas
- Ver su perfil con puntos, nivel y medallas
- Ver rankings

## ?? Archivos de Configuración

| Archivo | Descripción |
|---------|-------------|
| `Dockerfile` | Imagen Docker multi-stage |
| `.dockerignore` | Exclusiones para Docker |
| `docker-compose.yml` | Orquestación local |
| `.env.example` | Variables de entorno ejemplo |
| `deploy.ps1` | Script de despliegue local |
| `DEPLOYMENT.md` | Guía completa de despliegue |
| `ARQUITECTURA.md` | Documentación de arquitectura |

## ?? Testing

### Health Check
```bash
curl http://localhost:8080/health
```

Respuesta esperada:
```json
{
  "status": "Healthy",
  "results": {
    "database": {
      "status": "Healthy"
    }
  }
}
```

## ?? Documentación Adicional

- [?? Guía de Despliegue](DEPLOYMENT.md)
- [??? Arquitectura del Sistema](ARQUITECTURA.md)
- [?? Diagrama de Arquitectura](DIAGRAMA_ARQUITECTURA.md)
- [?? Resumen de Despliegue](RESUMEN_DESPLIEGUE.md)

## ?? Contribuir

1. Fork el proyecto
2. Crear una rama de feature (`git checkout -b feature/AmazingFeature`)
3. Commit los cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## ?? Licencia

Este proyecto es privado y confidencial.

## ?? Equipo

Desarrollado por el equipo DIARS

## ?? Enlaces Útiles

- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Coolify](https://coolify.io/)

---

? Si te gusta este proyecto, dale una estrella en GitHub!
