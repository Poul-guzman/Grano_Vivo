using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    /// <inheritdoc />
    public partial class piero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuiasEntrada",
                schema: "Identity",
                columns: table => new
                {
                    IdGuiaEntrada = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiasEntrada", x => x.IdGuiaEntrada);
                    table.ForeignKey(
                        name: "FK_GuiasEntrada_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "Identity",
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesGuiasEntrada",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdGuiaEntrada = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesGuiasEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesGuiasEntrada_GuiasEntrada_IdGuiaEntrada",
                        column: x => x.IdGuiaEntrada,
                        principalSchema: "Identity",
                        principalTable: "GuiasEntrada",
                        principalColumn: "IdGuiaEntrada",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesGuiasEntrada_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "Identity",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGuiasEntrada_IdGuiaEntrada",
                schema: "Identity",
                table: "DetallesGuiasEntrada",
                column: "IdGuiaEntrada");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGuiasEntrada_ProductoId",
                schema: "Identity",
                table: "DetallesGuiasEntrada",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_GuiasEntrada_ProveedorId",
                schema: "Identity",
                table: "GuiasEntrada",
                column: "ProveedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesGuiasEntrada",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "GuiasEntrada",
                schema: "Identity");
        }
    }
}
