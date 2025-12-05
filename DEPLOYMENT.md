# ?? Despliegue de RunaTalento en Coolify

## ?? Pre-requisitos

- ? Base de datos SQL Server desplegada en: `142.93.27.239:1433`
- ? Coolify instalado y configurado
- ? Git configurado en el servidor

---

## ?? Despliegue en Coolify

### Paso 1: Conectar el repositorio

1. En Coolify, crear un nuevo **Resource** ? **Application**
2. Seleccionar **GitHub/GitLab** como fuente
3. Conectar tu repositorio de Git
4. Seleccionar la rama principal (ej: `master` o `main`)

### Paso 2: Configurar el proyecto

En la configuración del proyecto en Coolify:

#### **Build Pack:**
- Seleccionar: `Dockerfile`

#### **Puerto:**
- Puerto del contenedor: `8080`

#### **Variables de Entorno:**
Agregar las siguientes variables en Coolify:

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Server=142.93.27.239,1433;Database=RunaTalentoDB;User Id=sa;Password=Sqlserver123!;TrustServerCertificate=True;
```

> **Nota:** En Coolify, usa `__` (doble guion bajo) para variables anidadas de configuración.

#### **Health Check:**
- URL: `/health`
- Intervalo: `30s`
- Timeout: `3s`

### Paso 3: Persistencia de archivos (Opcional)

Si necesitas persistir los uploads de estudiantes:

1. En Coolify, ir a **Volumes**
2. Agregar un volumen:
   - **Host Path:** `/var/coolify/uploads/runatalento`
   - **Container Path:** `/app/wwwroot/uploads`

### Paso 4: Desplegar

1. Click en **Deploy**
2. Coolify automáticamente:
   - Clonará el repositorio
   - Construirá la imagen Docker
   - Desplegará el contenedor
   - Configurará el proxy reverso

---

## ?? Pruebas Locales con Docker

Antes de desplegar en Coolify, puedes probar localmente:

### Construir la imagen:
```bash
docker build -t runatalento:latest .
```

### Ejecutar el contenedor:
```bash
docker run -d \
  -p 8080:8080 \
  --name runatalento \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=142.93.27.239,1433;Database=RunaTalentoDB;User Id=sa;Password=Sqlserver123!;TrustServerCertificate=True;" \
  runatalento:latest
```

### Probar la aplicación:
```bash
# Health check
curl http://localhost:8080/health

# Página principal
curl http://localhost:8080/
```

### Ver logs:
```bash
docker logs -f runatalento
```

### O usar Docker Compose:
```bash
docker-compose up -d
```

---

## ?? Configuración Avanzada

### Variables de Entorno Disponibles:

| Variable | Descripción | Valor por Defecto |
|----------|-------------|-------------------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | `Production` |
| `ASPNETCORE_URLS` | URLs de escucha | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a BD | (Ver appsettings.json) |
| `Logging__LogLevel__Default` | Nivel de log | `Information` |

### Personalizar appsettings.Production.json:

Si necesitas configuraciones específicas para producción, crea el archivo:
`RunaTalento/appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## ?? Monitoreo

### Health Check Endpoint:
```
GET /health
```

Respuesta exitosa:
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

### Logs en Coolify:
- Los logs están disponibles en la pestaña **Logs** del proyecto
- Nivel de logs configurado en `appsettings.json`

---

## ?? Seguridad

### Recomendaciones:

1. **Cambiar credenciales de BD:**
   - En producción, usa credenciales seguras
   - Considera usar Azure Key Vault o Variables de Entorno seguras

2. **HTTPS:**
   - Coolify maneja automáticamente SSL con Let's Encrypt
   - Configura tu dominio en Coolify

3. **Firewall:**
   - Asegúrate de que solo Coolify pueda acceder al puerto 8080
   - La BD debe estar protegida con reglas de firewall

4. **Secretos:**
   - Nunca commitees `appsettings.Production.json` con secretos
   - Usa variables de entorno en Coolify

---

## ?? Solución de Problemas

### Error: "Unable to connect to database"
```bash
# Verificar conectividad desde el contenedor
docker exec -it runatalento bash
apt-get update && apt-get install -y telnet
telnet 142.93.27.239 1433
```

### Error: "Port 8080 already in use"
```bash
# Verificar qué proceso usa el puerto
netstat -tulpn | grep 8080
# O en Windows
netstat -ano | findstr :8080
```

### Error: "Health check failed"
```bash
# Ver logs del contenedor
docker logs runatalento

# Verificar que la BD esté accesible
docker exec -it runatalento curl http://localhost:8080/health
```

### Reconstruir desde cero:
```bash
docker-compose down -v
docker system prune -a
docker-compose up --build -d
```

---

## ?? Checklist de Despliegue

- [ ] Base de datos SQL Server accesible
- [ ] Variables de entorno configuradas en Coolify
- [ ] Dockerfile en la raíz del proyecto
- [ ] `.dockerignore` configurado
- [ ] Health check endpoint funcionando
- [ ] Puerto 8080 expuesto
- [ ] SSL configurado en Coolify
- [ ] Dominio apuntando al servidor
- [ ] Backups de BD configurados
- [ ] Monitoreo activado

---

## ?? Soporte

Si encuentras problemas:
1. Revisa los logs en Coolify
2. Verifica la conectividad a la base de datos
3. Asegúrate de que las variables de entorno estén correctas
4. Prueba localmente con Docker primero

---

## ?? Recursos Adicionales

- [Documentación de Coolify](https://coolify.io/docs)
- [ASP.NET Core Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [Health Checks en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

---

## ? Estado del Despliegue

Última actualización: 05/12/2024

- [x] Dockerfile creado
- [x] .dockerignore configurado
- [x] Health checks implementados
- [x] Docker Compose para pruebas locales
- [x] Documentación de despliegue
- [ ] Desplegado en Coolify (pendiente)
