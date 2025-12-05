# ==============================================================================
# Script de Despliegue Local - RunaTalento
# Prueba la aplicación con Docker antes de desplegar en Coolify
# ==============================================================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("build", "run", "stop", "clean", "logs", "restart", "test")]
    [string]$Action = "run"
)

$ErrorActionPreference = "Stop"
$ImageName = "runatalento"
$ImageTag = "latest"
$ContainerName = "runatalento-app"
$Port = "8080"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RunaTalento - Despliegue Local" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

function Build-Image {
    Write-Host "?? Construyendo imagen Docker..." -ForegroundColor Yellow
    docker build -t "${ImageName}:${ImageTag}" .
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Imagen construida exitosamente" -ForegroundColor Green
    } else {
        Write-Host "? Error al construir la imagen" -ForegroundColor Red
        exit 1
    }
}

function Run-Container {
    Write-Host "?? Iniciando contenedor..." -ForegroundColor Yellow
    
    # Detener contenedor existente si existe
    $existing = docker ps -a -q -f name=$ContainerName
    if ($existing) {
        Write-Host "??  Deteniendo contenedor existente..." -ForegroundColor Yellow
        docker stop $ContainerName | Out-Null
        docker rm $ContainerName | Out-Null
    }
    
    # Ejecutar nuevo contenedor
    docker run -d `
        --name $ContainerName `
        -p "${Port}:8080" `
        -e ASPNETCORE_ENVIRONMENT=Production `
        -e "ConnectionStrings__DefaultConnection=Server=142.93.27.239,1433;Database=RunaTalentoDB;User Id=sa;Password=Sqlserver123!;TrustServerCertificate=True;" `
        "${ImageName}:${ImageTag}"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Contenedor iniciado exitosamente" -ForegroundColor Green
        Write-Host ""
        Write-Host "?? Aplicación disponible en: http://localhost:$Port" -ForegroundColor Cyan
        Write-Host "?? Health check: http://localhost:$Port/health" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Para ver los logs en tiempo real:" -ForegroundColor Yellow
        Write-Host "  docker logs -f $ContainerName" -ForegroundColor White
    } else {
        Write-Host "? Error al iniciar el contenedor" -ForegroundColor Red
        exit 1
    }
}

function Stop-Container {
    Write-Host "??  Deteniendo contenedor..." -ForegroundColor Yellow
    docker stop $ContainerName
    docker rm $ContainerName
    Write-Host "? Contenedor detenido" -ForegroundColor Green
}

function Clean-All {
    Write-Host "?? Limpiando recursos Docker..." -ForegroundColor Yellow
    
    # Detener y eliminar contenedor
    $existing = docker ps -a -q -f name=$ContainerName
    if ($existing) {
        docker stop $ContainerName | Out-Null
        docker rm $ContainerName | Out-Null
    }
    
    # Eliminar imagen
    docker rmi "${ImageName}:${ImageTag}" -f
    
    # Limpiar sistema (opcional)
    Write-Host "???  ¿Deseas limpiar imágenes y contenedores no utilizados? (s/n)" -ForegroundColor Yellow
    $response = Read-Host
    if ($response -eq "s") {
        docker system prune -a -f
    }
    
    Write-Host "? Limpieza completada" -ForegroundColor Green
}

function Show-Logs {
    Write-Host "?? Mostrando logs del contenedor..." -ForegroundColor Yellow
    docker logs -f $ContainerName
}

function Restart-Container {
    Write-Host "?? Reiniciando contenedor..." -ForegroundColor Yellow
    docker restart $ContainerName
    Write-Host "? Contenedor reiniciado" -ForegroundColor Green
}

function Test-Application {
    Write-Host "?? Probando la aplicación..." -ForegroundColor Yellow
    Write-Host ""
    
    # Esperar a que la aplicación inicie
    Write-Host "? Esperando 10 segundos para que la aplicación inicie..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    # Probar health check
    Write-Host "1?? Probando Health Check..." -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$Port/health" -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "   ? Health check OK" -ForegroundColor Green
        }
    } catch {
        Write-Host "   ? Health check FALLÓ" -ForegroundColor Red
    }
    
    # Probar página principal
    Write-Host "2?? Probando página principal..." -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$Port/" -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "   ? Página principal OK" -ForegroundColor Green
        }
    } catch {
        Write-Host "   ? Página principal FALLÓ" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "? Pruebas completadas" -ForegroundColor Green
}

# Ejecutar acción
switch ($Action) {
    "build" {
        Build-Image
    }
    "run" {
        Build-Image
        Run-Container
    }
    "stop" {
        Stop-Container
    }
    "clean" {
        Clean-All
    }
    "logs" {
        Show-Logs
    }
    "restart" {
        Restart-Container
    }
    "test" {
        Test-Application
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Operación completada" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
