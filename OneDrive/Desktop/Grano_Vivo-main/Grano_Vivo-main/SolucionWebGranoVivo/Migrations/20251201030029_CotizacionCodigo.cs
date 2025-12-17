using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class CotizacionCodigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                schema: "Identity",
                table: "Cotizaciones",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                schema: "Identity",
                table: "Cotizaciones");
        }
    }
}
