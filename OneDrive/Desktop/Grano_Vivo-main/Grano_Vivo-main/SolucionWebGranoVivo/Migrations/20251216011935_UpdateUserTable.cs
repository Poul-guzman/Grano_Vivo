using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolucionWebGranoVivo.Migrations
{
    public partial class UpdateUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportesCompraGenerados",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParametrosUsados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesCompraGenerados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesCompraGenerados_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalSchema: "Identity",
                        principalTable: "Proveedores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportesVentaGenerados",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParametrosUsados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesVentaGenerados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesVentaGenerados_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "Identity",
                        principalTable: "Clientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesCompraGenerados_ProveedorId",
                schema: "Identity",
                table: "ReportesCompraGenerados",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesVentaGenerados_ClienteId",
                schema: "Identity",
                table: "ReportesVentaGenerados",
                column: "ClienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesCompraGenerados",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ReportesVentaGenerados",
                schema: "Identity");
        }
    }
}
