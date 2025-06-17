using Inventario_Productos_Tecnologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Detectar automáticamente la instancia de SQL Server disponible
string GetAvailableConnection(IConfiguration configuration)
{
    var connections = new[] { "SQLExpress", "SQLServer", "LocalDB" };
    
    foreach (var conn in connections)
    {
        try
        {
            var connectionString = configuration.GetConnectionString(conn);
            if (string.IsNullOrEmpty(connectionString)) continue;
            
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return connectionString;
        }
        catch
        {
            continue;
        }
    }
    
    throw new Exception("No se encontró ninguna instancia de SQL Server disponible");
}

builder.Services.AddDbContext<TecnoCoreDbContext>(options => 
    options.UseSqlServer(GetAvailableConnection(builder.Configuration)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();