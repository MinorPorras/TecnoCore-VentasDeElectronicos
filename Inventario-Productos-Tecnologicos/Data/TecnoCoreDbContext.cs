using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Data;

public partial class TecnoCoreDbContext : IdentityDbContext<Usuarios, Roles, string>
{
    public TecnoCoreDbContext()
    {
    }

    public TecnoCoreDbContext(DbContextOptions<TecnoCoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorias> Categorias { get; set; }
    public virtual DbSet<Cupones> Cupones { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Direcciones> Direcciones { get; set; }

    public virtual DbSet<EstadosPedidos> EstadosPedidos { get; set; }
    public virtual DbSet<Kardex> Kardex { get; set; }

    public virtual DbSet<ListaDeseos> ListaDeseos { get; set; }

    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }

    public virtual DbSet<Pedidos> Pedidos { get; set; }
    public virtual DbSet<Marcas> Marcas { get; set; }
    public virtual DbSet<Productos> Productos { get; set; }
    public virtual DbSet<Subcategorias> Subcategorias { get; set; }
    public virtual DbSet<TipoMovimientoKardex> TipoMovimientoKardex { get; set; }

    public virtual DbSet<CarritoCompras> CarritoCompras { get; set; }

    public virtual DbSet<Provincia> Provincias { get; set; }

    public virtual DbSet<Canton> Cantones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .Build();
            var connectionString = configuration.GetConnectionString("defaultConn");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D5CFA9CB");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        builder.Entity<Cupones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cupones__3214EC0789F96FE7");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.UsosActuales).HasDefaultValue(0);
        });

        builder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetalleP__3214EC071DE766D1");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos);

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallePedidos);
        });

        builder.Entity<Direcciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Direccio__3214EC074E055094");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Usuario)
                .WithOne(u => u.Direccion)
                .HasForeignKey<Direcciones>(d => d.UsuarioId)
                .IsRequired(false);
        });

        builder.Entity<EstadosPedidos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosP__3214EC07B7070AF0");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        builder.Entity<Kardex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KARDEX__3214EC07886BCC24");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Producto).WithMany(p => p.Kardex);

            entity.HasOne(d => d.TipoMovimientoKardex).WithMany(p => p.Kardex);
        });

        builder.Entity<ListaDeseos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ListaDes__3214EC0724A5DC3E");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaAgregado).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Producto).WithMany(p => p.ListaDeseos);

            entity.HasOne(d => d.Usuario).WithMany(u => u.ListaDeseos).HasForeignKey(d => d.UsuarioId);
        });

        builder.Entity<MetodosPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MetodosP__3214EC07725674D8");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        builder.Entity<Pedidos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedidos__3214EC074497B4A2");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Cupon).WithMany(p => p.Pedidos);

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedidos);

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedidos);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos).HasForeignKey(d => d.UsuarioId);
        });

        builder.Entity<Productos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC076A3D152D");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Subcategoria).WithMany(p => p.Productos);
        });

        builder.Entity<Subcategorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subcateg__3214EC07A314AFA0");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Subcategorias);
        });

        builder.Entity<TipoMovimientoKardex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoMovi__3214EC07F8C6896C");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        builder.Entity<CarritoCompras>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.ProductoId });
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.CarritoCompras)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.CarritoCompras)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder builder);
}