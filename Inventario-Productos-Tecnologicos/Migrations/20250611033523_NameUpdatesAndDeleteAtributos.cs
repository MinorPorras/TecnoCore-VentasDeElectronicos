using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario_Productos_Tecnologicos.Migrations
{
    /// <inheritdoc />
    public partial class NameUpdatesAndDeleteAtributos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoAtributos");

            migrationBuilder.DropTable(
                name: "Atributos");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Roles",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "Atributos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    NombreAtributo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Atributo__3214EC07AB1F465C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductoAtributos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtributoId = table.Column<int>(type: "int", nullable: true),
                    ProductoId = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Valor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__3214EC0764D268DC", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ProductoA__Atrib__45F365D3",
                        column: x => x.AtributoId,
                        principalTable: "Atributos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ProductoA__Produ__44FF419A",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoAtributos_AtributoId",
                table: "ProductoAtributos",
                column: "AtributoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoAtributos_ProductoId",
                table: "ProductoAtributos",
                column: "ProductoId");
        }
    }
}
