using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class OrdenCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesCompra",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "Identity",
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrdenCompra",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenCompraId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrdenCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenCompra_OrdenesCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalSchema: "Identity",
                        principalTable: "OrdenesCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesOrdenCompra_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "Identity",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenCompra_OrdenCompraId",
                schema: "Identity",
                table: "DetallesOrdenCompra",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrdenCompra_ProductoId",
                schema: "Identity",
                table: "DetallesOrdenCompra",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_Codigo",
                schema: "Identity",
                table: "OrdenesCompra",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ProveedorId",
                schema: "Identity",
                table: "OrdenesCompra",
                column: "ProveedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesOrdenCompra",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "OrdenesCompra",
                schema: "Identity");
        }
    }
}
