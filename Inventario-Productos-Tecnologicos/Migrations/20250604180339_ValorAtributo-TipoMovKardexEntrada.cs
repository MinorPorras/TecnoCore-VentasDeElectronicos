using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario_Productos_Tecnologicos.Migrations
{
    /// <inheritdoc />
    public partial class ValorAtributoTipoMovKardexEntrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "TipoMovimientoKardex",
                newName: "Nombre");

            migrationBuilder.AddColumn<bool>(
                name: "Entrada",
                table: "TipoMovimientoKardex",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "Atributos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Entrada",
                table: "TipoMovimientoKardex");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Atributos");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "TipoMovimientoKardex",
                newName: "Tipo");
        }
    }
}
