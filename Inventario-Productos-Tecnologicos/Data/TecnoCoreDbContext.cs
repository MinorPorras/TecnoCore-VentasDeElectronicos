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

    public virtual DbSet<Atributo> Atributos { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cupone> Cupones { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Direccione> Direcciones { get; set; }

    public virtual DbSet<EstadosPedido> EstadosPedidos { get; set; }

    public virtual DbSet<Kardex> Kardices { get; set; }

    public virtual DbSet<ListaDeseo> ListaDeseos { get; set; }

    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoAtributo> ProductoAtributos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subcategoria> Subcategorias { get; set; }

    public virtual DbSet<TipoMovimientoKardex> TipoMovimientoKardices { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-J4DG90L\\SQLEXPRESS;Database=TecnoCoreDB;Integrated Security=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atributo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Atributo__3214EC07AB1F465C");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D5CFA9CB");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Cupone>(entity =>
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

        modelBuilder.Entity<Direccione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Direccio__3214EC074E055094");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Direcciones).HasConstraintName("FK__Direccion__Usuar__5629CD9C");
        });

        modelBuilder.Entity<EstadosPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosP__3214EC07B7070AF0");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Kardex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KARDEX__3214EC07886BCC24");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Producto).WithMany(p => p.Kardices).HasConstraintName("FK__KARDEX__Producto__72C60C4A");

            entity.HasOne(d => d.TipoMovimiento).WithMany(p => p.Kardices).HasConstraintName("FK__KARDEX__TipoMovi__73BA3083");
        });

        modelBuilder.Entity<ListaDeseo>(entity =>
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

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedidos__3214EC074497B4A2");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Cupon).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__CuponId__6754599E");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__EstadoP__66603565");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__MetodoP__656C112C");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos).HasConstraintName("FK__Pedidos__Usuario__6477ECF3");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC076A3D152D");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Subcategoria).WithMany(p => p.Productos).HasConstraintName("FK__Productos__Subca__3E52440B");
        });

        modelBuilder.Entity<ProductoAtributo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC0764D268DC");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Atributo).WithMany(p => p.ProductoAtributos).HasConstraintName("FK__ProductoA__Atrib__45F365D3");

            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoAtributos).HasConstraintName("FK__ProductoA__Produ__44FF419A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07CA43B814");

            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Subcategoria>(entity =>
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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07B49ACA59");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.Usuarios).HasConstraintName("FK__Usuarios__Rol__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
