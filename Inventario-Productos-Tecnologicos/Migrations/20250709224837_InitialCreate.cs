using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario_Productos_Tecnologicos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TECO_A_Role",
                columns: table => new
                {
                    TC_RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false),
                    TC_RoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TC_NormalizedRoleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TC_ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_A_Role", x => x.TC_RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TECO_A_User",
                columns: table => new
                {
                    TC_UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TC_Apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false),
                    TC_UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TC_NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TC_Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TC_NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TB_EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TC_PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TB_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TB_TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TF_LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TB_LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TN_AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_A_User", x => x.TC_UserId);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Categoria",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC07D5CFA9CB", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Cupon",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TC_TipoDescuento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TN_Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TF_FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    TF_FechaFin = table.Column<DateTime>(type: "datetime", nullable: false),
                    TN_UsosMaximos = table.Column<int>(type: "int", nullable: true),
                    TN_UsosActuales = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cupones__3214EC0789F96FE7", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_EstadoPedido",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_NombreEstado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EstadosP__3214EC07B7070AF0", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Marca",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_Marca", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_MetodosPago",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TB_NombreMetodo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MetodosP__3214EC07725674D8", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Provincia",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_Provincia", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_TipoMovimientoKardex",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TB_Entrada = table.Column<bool>(type: "bit", nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoMovi__3214EC07F8C6896C", x => x.TN_Id);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_RoleClaim",
                columns: table => new
                {
                    TN_RoleClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_RoleClaim", x => x.TN_RoleClaimId);
                    table.ForeignKey(
                        name: "FK_TECO_M_RoleClaim_TECO_A_Role_TC_RoleId",
                        column: x => x.TC_RoleId,
                        principalTable: "TECO_A_Role",
                        principalColumn: "TC_RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_UserClaim",
                columns: table => new
                {
                    TN_UserClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_UserClaim", x => x.TN_UserClaimId);
                    table.ForeignKey(
                        name: "FK_TECO_M_UserClaim_TECO_A_User_TC_UserId",
                        column: x => x.TC_UserId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_UserRole",
                columns: table => new
                {
                    TC_UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_UserRole", x => new { x.TC_UserId, x.TC_RoleId });
                    table.ForeignKey(
                        name: "FK_TECO_M_UserRole_TECO_A_Role_TC_RoleId",
                        column: x => x.TC_RoleId,
                        principalTable: "TECO_A_Role",
                        principalColumn: "TC_RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TECO_M_UserRole_TECO_A_User_TC_UserId",
                        column: x => x.TC_UserId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_UserToken",
                columns: table => new
                {
                    TC_UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TC_LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TC_TokenName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TC_TokenValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_UserToken", x => new { x.TC_UserId, x.TC_LoginProvider, x.TC_TokenName });
                    table.ForeignKey(
                        name: "FK_TECO_M_UserToken_TECO_A_User_TC_UserId",
                        column: x => x.TC_UserId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_UserLogin",
                columns: table => new
                {
                    TC_LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TC_ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TC_ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TC_UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_P_UserLogin", x => new { x.TC_LoginProvider, x.TC_ProviderKey });
                    table.ForeignKey(
                        name: "FK_TECO_P_UserLogin_TECO_A_User_TC_UserId",
                        column: x => x.TC_UserId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Subcategoria",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TN_CategoriaId = table.Column<int>(type: "int", nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subcateg__3214EC07A314AFA0", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_M_Subcategoria_TECO_M_Categoria_TN_CategoriaId",
                        column: x => x.TN_CategoriaId,
                        principalTable: "TECO_M_Categoria",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_Pedido",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TN_MetodoPagoId = table.Column<int>(type: "int", nullable: true),
                    TC_NumTarjeta = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TN_EstadoPedidoId = table.Column<int>(type: "int", nullable: true),
                    TN_TransaccionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TF_Fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    TN_CuponId = table.Column<int>(type: "int", nullable: true),
                    TN_Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TN_Impuesto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TN_Descuento = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TN_Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CuponId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedidos__3214EC074497B4A2", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_P_Pedido_TECO_A_User_TN_UsuarioId",
                        column: x => x.TN_UsuarioId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId");
                    table.ForeignKey(
                        name: "FK_TECO_P_Pedido_TECO_M_Cupon_CuponId",
                        column: x => x.CuponId,
                        principalTable: "TECO_M_Cupon",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_P_Pedido_TECO_M_EstadoPedido_TN_EstadoPedidoId",
                        column: x => x.TN_EstadoPedidoId,
                        principalTable: "TECO_M_EstadoPedido",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_P_Pedido_TECO_M_MetodosPago_TN_MetodoPagoId",
                        column: x => x.TN_MetodoPagoId,
                        principalTable: "TECO_M_MetodosPago",
                        principalColumn: "TN_Id");
                });

            migrationBuilder.CreateTable(
                name: "TECO_M_Canton",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TN_ProvinciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_M_Canton", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_M_Canton_TECO_M_Provincia_TN_ProvinciaId",
                        column: x => x.TN_ProvinciaId,
                        principalTable: "TECO_M_Provincia",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_A_Producto",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TN_Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TN_Stock = table.Column<int>(type: "int", nullable: false),
                    TC_Imagen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TB_Novedad = table.Column<bool>(type: "bit", nullable: false),
                    TN_MarcaId = table.Column<int>(type: "int", nullable: true),
                    TN_SubcategoriaId = table.Column<int>(type: "int", nullable: true),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__3214EC076A3D152D", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_A_Producto_TECO_M_Marca_TN_MarcaId",
                        column: x => x.TN_MarcaId,
                        principalTable: "TECO_M_Marca",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_A_Producto_TECO_M_Subcategoria_TN_SubcategoriaId",
                        column: x => x.TN_SubcategoriaId,
                        principalTable: "TECO_M_Subcategoria",
                        principalColumn: "TN_Id");
                });

            migrationBuilder.CreateTable(
                name: "TECO_A_Direccion",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TC_Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TN_CantonId = table.Column<int>(type: "int", nullable: false),
                    TC_CodigoPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Direccio__3214EC074E055094", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_A_Direccion_TECO_A_User_TN_UsuarioId",
                        column: x => x.TN_UsuarioId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId");
                    table.ForeignKey(
                        name: "FK_TECO_A_Direccion_TECO_M_Canton_TN_CantonId",
                        column: x => x.TN_CantonId,
                        principalTable: "TECO_M_Canton",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_CarritoCompras",
                columns: table => new
                {
                    TN_UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TN_ProductoId = table.Column<int>(type: "int", nullable: false),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: false),
                    TN_PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TECO_P_CarritoCompras", x => new { x.TN_UsuarioId, x.TN_ProductoId });
                    table.ForeignKey(
                        name: "FK_TECO_P_CarritoCompras_TECO_A_Producto_TN_ProductoId",
                        column: x => x.TN_ProductoId,
                        principalTable: "TECO_A_Producto",
                        principalColumn: "TN_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TECO_P_CarritoCompras_TECO_A_User_TN_UsuarioId",
                        column: x => x.TN_UsuarioId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_DetallePedido",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_PedidoId = table.Column<int>(type: "int", nullable: true),
                    TN_ProductoId = table.Column<int>(type: "int", nullable: true),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: true),
                    TN_PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleP__3214EC071DE766D1", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_P_DetallePedido_TECO_A_Producto_TN_ProductoId",
                        column: x => x.TN_ProductoId,
                        principalTable: "TECO_A_Producto",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_P_DetallePedido_TECO_P_Pedido_TN_PedidoId",
                        column: x => x.TN_PedidoId,
                        principalTable: "TECO_P_Pedido",
                        principalColumn: "TN_Id");
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_Kardex",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_ProductoId = table.Column<int>(type: "int", nullable: true),
                    TN_Cantidad = table.Column<int>(type: "int", nullable: true),
                    TC_Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TF_Fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    TN_StockAnterior = table.Column<int>(type: "int", nullable: true),
                    TN_StockActual = table.Column<int>(type: "int", nullable: true),
                    TN_TipoMovimientoId = table.Column<int>(type: "int", nullable: true),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KARDEX__3214EC07886BCC24", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_P_Kardex_TECO_A_Producto_TN_ProductoId",
                        column: x => x.TN_ProductoId,
                        principalTable: "TECO_A_Producto",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_P_Kardex_TECO_M_TipoMovimientoKardex_TN_TipoMovimientoId",
                        column: x => x.TN_TipoMovimientoId,
                        principalTable: "TECO_M_TipoMovimientoKardex",
                        principalColumn: "TN_Id");
                });

            migrationBuilder.CreateTable(
                name: "TECO_P_ListaDeseos",
                columns: table => new
                {
                    TN_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TN_UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TN_ProductoId = table.Column<int>(type: "int", nullable: true),
                    TF_FechaAgregado = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TB_Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ListaDes__3214EC0724A5DC3E", x => x.TN_Id);
                    table.ForeignKey(
                        name: "FK_TECO_P_ListaDeseos_TECO_A_Producto_TN_ProductoId",
                        column: x => x.TN_ProductoId,
                        principalTable: "TECO_A_Producto",
                        principalColumn: "TN_Id");
                    table.ForeignKey(
                        name: "FK_TECO_P_ListaDeseos_TECO_A_User_TN_UsuarioId",
                        column: x => x.TN_UsuarioId,
                        principalTable: "TECO_A_User",
                        principalColumn: "TC_UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TECO_A_Direccion_TN_CantonId",
                table: "TECO_A_Direccion",
                column: "TN_CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_A_Direccion_TN_UsuarioId",
                table: "TECO_A_Direccion",
                column: "TN_UsuarioId",
                unique: true,
                filter: "[TN_UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_A_Producto_TN_MarcaId",
                table: "TECO_A_Producto",
                column: "TN_MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_A_Producto_TN_SubcategoriaId",
                table: "TECO_A_Producto",
                column: "TN_SubcategoriaId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "TECO_A_Role",
                column: "TC_NormalizedRoleName",
                unique: true,
                filter: "[TC_NormalizedRoleName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "TECO_A_User",
                column: "TC_NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "TECO_A_User",
                column: "TC_NormalizedUserName",
                unique: true,
                filter: "[TC_NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_M_Canton_TN_ProvinciaId",
                table: "TECO_M_Canton",
                column: "TN_ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "UQ__Cupones__06370DACEA3BF6E0",
                table: "TECO_M_Cupon",
                column: "TC_Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TECO_M_RoleClaim_TC_RoleId",
                table: "TECO_M_RoleClaim",
                column: "TC_RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_M_Subcategoria_TN_CategoriaId",
                table: "TECO_M_Subcategoria",
                column: "TN_CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_M_UserClaim_TC_UserId",
                table: "TECO_M_UserClaim",
                column: "TC_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_M_UserRole_TC_RoleId",
                table: "TECO_M_UserRole",
                column: "TC_RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_CarritoCompras_TN_ProductoId",
                table: "TECO_P_CarritoCompras",
                column: "TN_ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_DetallePedido_TN_PedidoId",
                table: "TECO_P_DetallePedido",
                column: "TN_PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_DetallePedido_TN_ProductoId",
                table: "TECO_P_DetallePedido",
                column: "TN_ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Kardex_TN_ProductoId",
                table: "TECO_P_Kardex",
                column: "TN_ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Kardex_TN_TipoMovimientoId",
                table: "TECO_P_Kardex",
                column: "TN_TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_ListaDeseos_TN_ProductoId",
                table: "TECO_P_ListaDeseos",
                column: "TN_ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_ListaDeseos_TN_UsuarioId",
                table: "TECO_P_ListaDeseos",
                column: "TN_UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Pedido_CuponId",
                table: "TECO_P_Pedido",
                column: "CuponId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Pedido_TN_EstadoPedidoId",
                table: "TECO_P_Pedido",
                column: "TN_EstadoPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Pedido_TN_MetodoPagoId",
                table: "TECO_P_Pedido",
                column: "TN_MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_Pedido_TN_UsuarioId",
                table: "TECO_P_Pedido",
                column: "TN_UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TECO_P_UserLogin_TC_UserId",
                table: "TECO_P_UserLogin",
                column: "TC_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TECO_A_Direccion");

            migrationBuilder.DropTable(
                name: "TECO_M_RoleClaim");

            migrationBuilder.DropTable(
                name: "TECO_M_UserClaim");

            migrationBuilder.DropTable(
                name: "TECO_M_UserRole");

            migrationBuilder.DropTable(
                name: "TECO_M_UserToken");

            migrationBuilder.DropTable(
                name: "TECO_P_CarritoCompras");

            migrationBuilder.DropTable(
                name: "TECO_P_DetallePedido");

            migrationBuilder.DropTable(
                name: "TECO_P_Kardex");

            migrationBuilder.DropTable(
                name: "TECO_P_ListaDeseos");

            migrationBuilder.DropTable(
                name: "TECO_P_UserLogin");

            migrationBuilder.DropTable(
                name: "TECO_M_Canton");

            migrationBuilder.DropTable(
                name: "TECO_A_Role");

            migrationBuilder.DropTable(
                name: "TECO_P_Pedido");

            migrationBuilder.DropTable(
                name: "TECO_M_TipoMovimientoKardex");

            migrationBuilder.DropTable(
                name: "TECO_A_Producto");

            migrationBuilder.DropTable(
                name: "TECO_M_Provincia");

            migrationBuilder.DropTable(
                name: "TECO_A_User");

            migrationBuilder.DropTable(
                name: "TECO_M_Cupon");

            migrationBuilder.DropTable(
                name: "TECO_M_EstadoPedido");

            migrationBuilder.DropTable(
                name: "TECO_M_MetodosPago");

            migrationBuilder.DropTable(
                name: "TECO_M_Marca");

            migrationBuilder.DropTable(
                name: "TECO_M_Subcategoria");

            migrationBuilder.DropTable(
                name: "TECO_M_Categoria");
        }
    }
}
