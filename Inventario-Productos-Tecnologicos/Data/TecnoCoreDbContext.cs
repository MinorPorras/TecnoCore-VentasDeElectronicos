using System;
using System.Collections.Generic;
using Inventario_Productos_Tecnologicos.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Data;

public partial class TecnoCoreDbContext : DbContext
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
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<Subcategorias> Subcategorias { get; set; }

    public virtual DbSet<TipoMovimientoKardex> TipoMovimientoKardex { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .Build();
            var connectionString = configuration.GetConnectionString("defaultConn");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D5CFA9CB");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Cupones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cupones__3214EC0789F96FE7");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.UsosActuales).HasDefaultValue(0);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetalleP__3214EC071DE766D1");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos).HasConstraintName("FK__DetallePe__Pedid__6B24EA82");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallePedidos).HasConstraintName("FK__DetallePe__Produ__6C190EBB");
        });

        modelBuilder.Entity<Direcciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Direccio__3214EC074E055094");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Direcciones).HasConstraintName("FK__Direccion__Usuar__5629CD9C");
        });

        modelBuilder.Entity<EstadosPedidos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosP__3214EC07B7070AF0");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Kardex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KARDEX__3214EC07886BCC24");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Producto).WithMany(p => p.Kardex).HasConstraintName("FK__KARDEX__Producto__72C60C4A");

            entity.HasOne(d => d.TipoMovimientoKardex).WithMany(p => p.Kardex).HasConstraintName("FK__KARDEX__TipoMovi__73BA3083");
        });

        modelBuilder.Entity<ListaDeseos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ListaDes__3214EC0724A5DC3E");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaAgregado).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Producto).WithMany(p => p.ListaDeseos).HasConstraintName("FK__ListaDese__Produ__5165187F");

            entity.HasOne(d => d.Usuario).WithMany(p => p.ListaDeseos).HasConstraintName("FK__ListaDese__Usuar__5070F446");
        });

        modelBuilder.Entity<MetodosPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MetodosP__3214EC07725674D8");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Pedidos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedidos__3214EC074497B4A2");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Cupon).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__CuponId__6754599E");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__EstadoP__66603565");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__MetodoP__656C112C");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__Usuario__6477ECF3");
        });

        modelBuilder.Entity<Productos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC076A3D152D");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Subcategoria).WithMany(p => p.Productos).HasConstraintName("FK__Productos__Subca__3E52440B");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07CA43B814");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Subcategorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subcateg__3214EC07A314AFA0");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Subcategorias).HasConstraintName("FK__Subcatego__Categ__3A81B327");
        });

        modelBuilder.Entity<TipoMovimientoKardex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoMovi__3214EC07F8C6896C");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07B49ACA59");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.Usuarios).HasConstraintName("FK__Usuarios__Rol__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
