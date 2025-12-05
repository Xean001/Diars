# ? Comandos Rápidos - RunaTalento

## ?? Docker Local

### Comandos Básicos
```powershell
# Construir y ejecutar
.\deploy.ps1 -Action run

# Solo construir imagen
.\deploy.ps1 -Action build

# Ver logs en tiempo real
.\deploy.ps1 -Action logs

# Probar la aplicación
.\deploy.ps1 -Action test

# Reiniciar contenedor
.\deploy.ps1 -Action restart

# Detener contenedor
.\deploy.ps1 -Action stop

# Limpiar todo
.\deploy.ps1 -Action clean
```

### Docker Compose
```bash
# Iniciar
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener
docker-compose down

# Reconstruir
docker-compose up --build -d

# Ver estado
docker-compose ps
```

### Comandos Docker Manuales
```bash
# Construir imagen
docker build -t runatalento:latest .

# Ejecutar contenedor
docker run -d -p 8080:8080 --name runatalento runatalento:latest

# Ver logs
docker logs -f runatalento

# Entrar al contenedor
docker exec -it runatalento bash

# Detener y eliminar
docker stop runatalento && docker rm runatalento

# Ver contenedores
docker ps -a

# Limpiar todo
docker system prune -a
```

## ?? Desarrollo Local (sin Docker)

### .NET CLI
```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con watch (recarga automática)
dotnet watch run

# Limpiar
dotnet clean

# Aplicar migraciones
dotnet ef database update

# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Revertir última migración
dotnet ef migrations remove
```

## ?? Testing

### Health Check
```bash
# PowerShell
Invoke-WebRequest http://localhost:8080/health

# Bash/Linux
curl http://localhost:8080/health

# Con formato bonito (requiere jq)
curl http://localhost:8080/health | jq
```

### Página Principal
```bash
# PowerShell
Invoke-WebRequest http://localhost:8080/

# Bash/Linux
curl http://localhost:8080/
```

## ?? Git

### Comandos Básicos
```bash
# Ver estado
git status

# Agregar archivos de despliegue
git add Dockerfile .dockerignore docker-compose.yml DEPLOYMENT.md README.md

# Commit
git commit -m "Add Docker deployment configuration"

# Push
git push origin main

# Ver cambios
git diff

# Ver log
git log --oneline -10
```

## ?? Entity Framework

### Migraciones
```bash
# Ver migraciones
dotnet ef migrations list

# Crear migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir a migración específica
dotnet ef database update NombreMigracion

# Eliminar última migración
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script

# Ver conexión
dotnet ef dbcontext info
```

## ?? Coolify

### Despliegue Manual (via CLI de Coolify)
```bash
# Conectar via SSH al servidor Coolify
ssh user@coolify-server

# Ver logs de la aplicación
coolify logs runatalento

# Reiniciar aplicación
coolify restart runatalento

# Ver estado
coolify status runatalento
```

### Variables de Entorno en Coolify
```bash
# Formato para agregar en Coolify UI
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Server=142.93.27.239,1433;Database=RunaTalentoDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;
```

## ?? Diagnóstico

### Ver Logs
```bash
# Docker
docker logs -f runatalento

# .NET (Development)
# Los logs aparecen en la consola al ejecutar dotnet run

# Windows Event Viewer
eventvwr.msc
```

### Verificar Puerto
```bash
# PowerShell (Windows)
netstat -ano | findstr :8080

# Bash (Linux)
netstat -tulpn | grep 8080
```

### Verificar Conectividad a BD
```bash
# Usando telnet (Windows)
telnet 142.93.27.239 1433

# Usando PowerShell
Test-NetConnection -ComputerName 142.93.27.239 -Port 1433

# Desde contenedor Docker
docker exec -it runatalento bash
apt-get update && apt-get install -y telnet
telnet 142.93.27.239 1433
```

### Verificar Imagen Docker
```bash
# Ver imágenes
docker images

# Ver tamaño
docker images runatalento

# Inspeccionar imagen
docker inspect runatalento:latest

# Ver historial de capas
docker history runatalento:latest
```

## ?? Limpieza

### Limpiar Archivos .NET
```bash
# Limpiar build
dotnet clean

# Eliminar carpetas bin y obj
Remove-Item -Recurse -Force bin,obj
```

### Limpiar Docker
```bash
# Detener todos los contenedores
docker stop $(docker ps -aq)

# Eliminar todos los contenedores
docker rm $(docker ps -aq)

# Eliminar imágenes sin usar
docker image prune -a

# Limpiar todo (¡CUIDADO!)
docker system prune -a --volumes
```

## ?? Monitoreo

### Ver Estado de Servicios
```bash
# Docker
docker ps
docker stats runatalento

# Coolify
# Ver en la UI: Dashboard ? Application ? Logs/Metrics
```

### Health Checks
```bash
# Verificar health check cada 5 segundos
while ($true) { 
    Invoke-WebRequest http://localhost:8080/health | Select-Object StatusCode, StatusDescription
    Start-Sleep -Seconds 5 
}
```

## ?? Despliegue Rápido

### En Desarrollo
```bash
dotnet run
```

### En Producción (Docker Local)
```powershell
.\deploy.ps1 -Action run
```

### En Coolify
```bash
# 1. Push a Git
git push origin main

# 2. Coolify detectará cambios y desplegará automáticamente
# O manualmente en la UI: Click en "Deploy"
```

## ?? Notas

- Puerto local: `8080`
- Puerto HTTPS local (dev): `7224`
- Base de datos: `142.93.27.239:1433`
- Health check: `/health`

## ?? Ayuda Rápida

```powershell
# Ver ayuda del script
Get-Help .\deploy.ps1

# Ver parámetros disponibles
Get-Help .\deploy.ps1 -Parameter Action
```

## ?? URLs Importantes

- **Local (Dev)**: https://localhost:7224
- **Local (Docker)**: http://localhost:8080
- **Health Check**: http://localhost:8080/health
- **Login**: http://localhost:8080/Identity/Account/Login

---

?? **Tip**: Guarda este archivo como referencia rápida para comandos comunes.
