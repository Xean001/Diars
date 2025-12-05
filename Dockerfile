# ==============================================================================
# Dockerfile para RunaTalento - ASP.NET Core 8.0 MVC + Razor Pages
# Optimizado para despliegue en Coolify
# ==============================================================================

# Etapa 1: Build - Compilación de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["RunaTalento/RunaTalento.csproj", "RunaTalento/"]
RUN dotnet restore "RunaTalento/RunaTalento.csproj"

# Copiar todo el código fuente
COPY . .
WORKDIR "/src/RunaTalento"

# Compilar la aplicación en modo Release
RUN dotnet build "RunaTalento.csproj" -c Release -o /app/build

# Etapa 2: Publish - Publicar la aplicación
FROM build AS publish
RUN dotnet publish "RunaTalento.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Runtime - Imagen final para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar dependencias del sistema (opcional, para diagnóstico)
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copiar archivos publicados desde la etapa de publish
COPY --from=publish /app/publish .

# Crear directorio para uploads si no existe
RUN mkdir -p /app/wwwroot/uploads/actividades && \
    chmod -R 755 /app/wwwroot/uploads

# Configurar variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Exponer puerto 8080 (Coolify usa este puerto por defecto)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Usuario no privilegiado para seguridad
USER $APP_UID

# Comando de inicio
ENTRYPOINT ["dotnet", "RunaTalento.dll"]
