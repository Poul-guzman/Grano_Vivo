using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class piero2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoUnitario",
                schema: "Identity",
                table: "DetallesGuiasEntrada",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoUnitario",
                schema: "Identity",
                table: "DetallesGuiasEntrada");
        }
    }
}
