using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;
using RunaTalento.Middleware;
// ✅ IMPORTAR SERVICIOS Y LÓGICA DE NEGOCIO (Arquitectura en capas)
using RunaTalento.BusinessLogic;
using RunaTalento.Services;
using RunaTalento.Services.Factories;
using RunaTalento.Services.Strategies;
using RunaTalento.Services.Observers;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURACIÓN DE SERVICIOS ==========

// Agregar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar ASP.NET Identity con roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ✅ CAPA DE LÓGICA DE NEGOCIO (Business Logic Layer)
// Patrón CONTROLLER (GRASP) - Coordinador de lógica de negocio
builder.Services.AddScoped<CalificacionBusinessLogic>();

// ✅ CAPA DE SERVICIOS (Service Layer)
// Patrón HIGH COHESION (GRASP) - Servicios especializados
builder.Services.AddScoped<GamificacionService>();

// ✅ PATRONES DE DISEÑO - CREACIONALES
// Patrón FACTORY METHOD (GoF) - Creación de estrategias
builder.Services.AddScoped<ICalificacionStrategyFactory, CalificacionStrategyFactory>();

// ✅ PATRONES DE DISEÑO - COMPORTAMIENTO
// Patrón STRATEGY (GoF) - Por defecto usa estrategia estándar
builder.Services.AddScoped<ICalificacionStrategy>(provider => 
    new CalificacionEstandarStrategy());

// Patrón OBSERVER (GoF) - Sistema de notificaciones
builder.Services.AddSingleton<CalificacionNotifier>();
builder.Services.AddScoped<ICalificacionObserver, LogCalificacionObserver>();

// ✅ Health Checks para Coolify
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

// Agregar soporte para Razor Pages (necesario para Identity UI)
builder.Services.AddRazorPages();

// Agregar controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========== APLICAR MIGRACIONES Y SEEDEAR DATOS ==========
// Inicializar la base de datos automáticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // ✅ Aplicar migraciones pendientes automáticamente (Code First)
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
        
        // ✅ Crear base de datos si no existe
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones");
    }
}

// ========== CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE ==========

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: The default HSTS value is 30 days
    app.UseHsts();
}

// ✅ Health Check Endpoint para Coolify (debe estar antes de UseHttpsRedirection)
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: UseAuthentication debe ir antes de UseAuthorization
app.UseAuthentication();

// ✅ AGREGAR MIDDLEWARE PARA VERIFICAR ESTADO DEL USUARIO
app.UseCheckUserActive();

app.UseAuthorization();

// Mapear rutas de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear Razor Pages (necesario para Identity)
app.MapRazorPages();

// ========== INICIALIZACIÓN DE ROLES Y USUARIO ADMINISTRADOR ==========
// Crear roles predeterminados si no existen
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        
        // ✅ Crear roles si no existen
        string[] roleNames = { "Admin", "Docente", "Estudiante" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // ✅ Crear usuario administrador por defecto
        var adminEmail = "admin@runatalento.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombres = "Administrador",
                Apellidos = "Sistema",
                EmailConfirmed = true,
                Estado = "Activo",
                FechaRegistro = DateTime.Now,
                PuntajeTotal = 0
            };
            
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al crear roles o usuario administrador");
    }
}

app.Run();
