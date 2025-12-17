using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class IntentoCoti1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                schema: "Identity",
                table: "Cotizaciones",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Codigo",
                schema: "Identity",
                table: "Cotizaciones",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_Codigo",
                schema: "Identity",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Codigo",
                schema: "Identity",
                table: "Cotizaciones");
        }
    }
}
