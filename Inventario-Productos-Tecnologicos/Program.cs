using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TecnoCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLExpress")));

builder.Services.AddDefaultIdentity<TECO_A_Usuario>(options =>
    {
        //Opciones de contraseñas
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;

        //Opciones de bloqueo de cuenta
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        //Opciones de usuario
        options.User.RequireUniqueEmail = true;

        //Opciones de SingIn
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<TECO_A_Roles>()
    .AddEntityFrameworkStores<TecnoCoreDbContext>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//Inicializa la base de datos del proyecto
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await DbInitializer.Initialize(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();

// Se crea un proveedor de tipos de contenido para poder añadir nuevos.
var provider = new FileExtensionContentTypeProvider
{
    Mappings =
    {
        // Se añade el mapeo para la extensión .avif, asociándola con el tipo MIME image/avif.
        [".avif"] = "image/avif"
    }
};

// Se aplican las opciones al middleware de archivos estáticos.
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();