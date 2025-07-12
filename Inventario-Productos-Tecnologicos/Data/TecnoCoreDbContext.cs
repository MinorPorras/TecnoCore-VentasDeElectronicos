using Inventario_Productos_Tecnologicos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos.Data;

public partial class TecnoCoreDbContext : IdentityDbContext<TECO_A_Usuario, TECO_A_Roles, string>
{
    public TecnoCoreDbContext()
    {
    }

    public TecnoCoreDbContext(DbContextOptions<TecnoCoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TECO_M_Categoria> TECO_M_Categoria { get; set; }
    public virtual DbSet<TECO_M_Cupon> TECO_M_Cupon { get; set; }

    public virtual DbSet<TECO_P_DetallePedido> TECO_P_DetallePedido { get; set; }

    public virtual DbSet<TECO_A_Direccion> TECO_A_Direccion { get; set; }

    public virtual DbSet<TECO_M_EstadoPedido> TECO_M_EstadoPedido { get; set; }
    public virtual DbSet<TECO_P_Kardex> TECO_P_Kardex { get; set; }

    public virtual DbSet<TECO_P_ListaDeseos> TECO_P_ListaDeseos { get; set; }

    public virtual DbSet<TECO_M_MetodoPago> TECO_M_MetodosPago { get; set; }

    public virtual DbSet<TECO_P_Pedido> TECO_P_Pedido { get; set; }
    public virtual DbSet<TECO_M_Marca> TECO_M_Marca { get; set; }
    public virtual DbSet<TECO_A_Producto> TECO_A_Producto { get; set; }
    public virtual DbSet<TECO_M_Subcategoria> TECO_M_Subcategoria { get; set; }
    public virtual DbSet<TECO_M_TipoMovimientoKardex> TECO_M_TipoMovimientoKardex { get; set; }

    public virtual DbSet<TECO_P_CarritoCompras> TECO_P_CarritoCompras { get; set; }

    public virtual DbSet<TECO_M_Provincia> TECO_M_Provincia { get; set; }

    public virtual DbSet<TECO_M_Canton> TECO_M_Canton { get; set; }

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


        // Tabla de Usuarios (TECO_A_User) - Clasificada como 'Actual'
        builder.Entity<TECO_A_Usuario>(entity =>
        {
            entity.ToTable("TECO_A_User"); // Renombrada a TECO_A_User

            // Estándar de Nombres de Campos T<Tipo>_nombre
            entity.Property(e => e.Id).HasColumnName("TC_UserId");
            entity.Property(e => e.UserName).HasColumnName("TC_UserName");
            entity.Property(e => e.NormalizedUserName).HasColumnName("TC_NormalizedUserName");
            entity.Property(e => e.Email).HasColumnName("TC_Email");
            entity.Property(e => e.NormalizedEmail).HasColumnName("TC_NormalizedEmail");
            entity.Property(e => e.EmailConfirmed).HasColumnName("TB_EmailConfirmed");
            entity.Property(e => e.PasswordHash).HasColumnName("TC_PasswordHash");
            entity.Property(e => e.SecurityStamp).HasColumnName("TC_SecurityStamp");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("TC_ConcurrencyStamp");
            entity.Property(e => e.PhoneNumber).HasColumnName("TC_PhoneNumber");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("TB_PhoneNumberConfirmed");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("TB_TwoFactorEnabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("TF_LockoutEnd");
            entity.Property(e => e.LockoutEnabled).HasColumnName("TB_LockoutEnabled");
            entity.Property(e => e.AccessFailedCount).HasColumnName("TN_AccessFailedCount");
        });

        // Tabla de Roles (TECO_A_Role) - Clasificada como 'Actual'
        builder.Entity<TECO_A_Roles>(entity =>
        {
            entity.ToTable("TECO_A_Role"); // Renombrada a TECO_A_Role

            // Estándar de Nombres de Campos
            entity.Property(e => e.Id).HasColumnName("TC_RoleId");
            entity.Property(e => e.Name).HasColumnName("TC_RoleName");
            entity.Property(e => e.NormalizedName).HasColumnName("TC_NormalizedRoleName");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("TC_ConcurrencyStamp");
        });

        // Tabla de Claims de Usuarios (TECO_M_UserClaim) - Clasificada como 'Mantenimiento'
        // Sirve para mantener propiedades o afirmaciones sobre el usuario
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("TECO_M_UserClaim"); // Renombrada a TECO_M_UserClaim

            // Estándar de Nombres de Campos
            entity.Property(e => e.Id).HasColumnName("TN_UserClaimId");
            entity.Property(e => e.UserId).HasColumnName("TC_UserId");
            entity.Property(e => e.ClaimType).HasColumnName("TC_ClaimType");
            entity.Property(e => e.ClaimValue).HasColumnName("TC_ClaimValue");
        });

        // Tabla de Logins de Usuarios (TECO_P_UserLogin) - Clasificada como 'Proceso'
        // Registra el proceso de cómo un usuario inició sesión
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("TECO_P_UserLogin"); // Renombrada a TECO_P_UserLogin

            // Estándar de Nombres de Campos
            entity.Property(e => e.LoginProvider).HasColumnName("TC_LoginProvider");
            entity.Property(e => e.ProviderKey).HasColumnName("TC_ProviderKey");
            entity.Property(e => e.ProviderDisplayName).HasColumnName("TC_ProviderDisplayName");
            entity.Property(e => e.UserId).HasColumnName("TC_UserId");
        });

        // Tabla de Tokens de Usuarios (TECO_M_UserToken) - Clasificada como 'Mantenimiento'
        // Se usa para mantener tokens de seguridad asociados al usuario
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("TECO_M_UserToken"); // Renombrada a TECO_M_UserToken

            // Estándar de Nombres de Campos
            entity.Property(e => e.UserId).HasColumnName("TC_UserId");
            entity.Property(e => e.LoginProvider).HasColumnName("TC_LoginProvider");
            entity.Property(e => e.Name).HasColumnName("TC_TokenName");
            entity.Property(e => e.Value).HasColumnName("TC_TokenValue");
        });

        // Tabla de Claims de Roles (TECO_M_RoleClaim) - Clasificada como 'Mantenimiento'
        // Similar a User Claims, pero para propiedades o afirmaciones sobre roles
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("TECO_M_RoleClaim"); // Renombrada a TECO_M_RoleClaim

            // Estándar de Nombres de Campos
            entity.Property(e => e.Id).HasColumnName("TN_RoleClaimId");
            entity.Property(e => e.RoleId).HasColumnName("TC_RoleId");
            entity.Property(e => e.ClaimType).HasColumnName("TC_ClaimType");
            entity.Property(e => e.ClaimValue).HasColumnName("TC_ClaimValue");
        });

        // Tabla de Relación Usuario-Rol (TECO_M_UserRole) - Clasificada como 'Mantenimiento'
        // Mantiene la asignación de roles a usuarios
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("TECO_M_UserRole"); // Renombrada a TECO_M_UserRole

            // Estándar de Nombres de Campos
            entity.Property(e => e.UserId).HasColumnName("TC_UserId");
            entity.Property(e => e.RoleId).HasColumnName("TC_RoleId");
            // Nota: IdentityUserRole tiene una clave compuesta por UserId y RoleId, no un 'Id' simple
        });

        builder.Entity<TECO_M_Categoria>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Categori__3214EC07D5CFA9CB");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
        });

        builder.Entity<TECO_M_Cupon>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Cupones__3214EC0789F96FE7");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
            entity.Property(e => e.TN_UsosActuales).HasDefaultValue(0);
        });

        builder.Entity<TECO_P_DetallePedido>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__DetalleP__3214EC071DE766D1");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos);

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallePedidos);
        });

        builder.Entity<TECO_A_Direccion>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Direccio__3214EC074E055094");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Usuario)
                .WithOne(u => u.Direccion)
                .HasForeignKey<TECO_A_Direccion>(d => d.TN_UsuarioId)
                .IsRequired(false);
        });

        builder.Entity<TECO_M_EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__EstadosP__3214EC07B7070AF0");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
        });

        builder.Entity<TECO_P_Kardex>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__KARDEX__3214EC07886BCC24");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Producto).WithMany(p => p.Kardex);

            entity.HasOne(d => d.TipoMovimientoKardex).WithMany(p => p.Kardex);
        });

        builder.Entity<TECO_P_ListaDeseos>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__ListaDes__3214EC0724A5DC3E");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
            entity.Property(e => e.TF_FechaAgregado).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Producto).WithMany(p => p.ListaDeseos);

            entity.HasOne(d => d.Usuario).WithMany(u => u.ListaDeseos).HasForeignKey(d => d.TN_UsuarioId);
        });

        builder.Entity<TECO_M_MetodoPago>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__MetodosP__3214EC07725674D8");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
        });

        builder.Entity<TECO_P_Pedido>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Pedidos__3214EC074497B4A2");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Cupon).WithMany(p => p.Pedido);

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedido);

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedido);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedido).HasForeignKey(d => d.TN_UsuarioId);
        });

        builder.Entity<TECO_A_Producto>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Producto__3214EC076A3D152D");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Subcategoria).WithMany(p => p.Productos);
        });

        builder.Entity<TECO_M_Subcategoria>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__Subcateg__3214EC07A314AFA0");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Subcategoria);
        });

        builder.Entity<TECO_M_TipoMovimientoKardex>(entity =>
        {
            entity.HasKey(e => e.TN_Id).HasName("PK__TipoMovi__3214EC07F8C6896C");

            entity.Property(e => e.TB_Activo).HasDefaultValue(true);
        });

        builder.Entity<TECO_P_CarritoCompras>(entity =>
        {
            entity.HasKey(e => new { e.TN_UsuarioId, e.TN_ProductoId });
            entity.Property(e => e.TN_PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.CarritoCompras)
                .HasForeignKey(d => d.TN_UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.CarritoCompras)
                .HasForeignKey(d => d.TN_ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder builder);
}