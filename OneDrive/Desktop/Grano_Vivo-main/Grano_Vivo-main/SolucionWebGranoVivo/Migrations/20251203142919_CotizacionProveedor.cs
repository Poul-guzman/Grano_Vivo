using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class CotizacionProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Clientes_ClienteId",
                schema: "Identity",
                table: "Cotizaciones");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                schema: "Identity",
                table: "Cotizaciones",
                newName: "ProveedorId");

            migrationBuilder.RenameIndex(
                name: "IX_Cotizaciones_ClienteId",
                schema: "Identity",
                table: "Cotizaciones",
                newName: "IX_Cotizaciones_ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Proveedores_ProveedorId",
                schema: "Identity",
                table: "Cotizaciones",
                column: "ProveedorId",
                principalSchema: "Identity",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Proveedores_ProveedorId",
                schema: "Identity",
                table: "Cotizaciones");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                schema: "Identity",
                table: "Cotizaciones",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Cotizaciones_ProveedorId",
                schema: "Identity",
                table: "Cotizaciones",
                newName: "IX_Cotizaciones_ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Clientes_ClienteId",
                schema: "Identity",
                table: "Cotizaciones",
                column: "ClienteId",
                principalSchema: "Identity",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
