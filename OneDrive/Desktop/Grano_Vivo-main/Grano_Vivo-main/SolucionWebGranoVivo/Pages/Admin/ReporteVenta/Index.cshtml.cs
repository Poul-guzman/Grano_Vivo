using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SolucionWebGranoVivo.Pages.Admin.ReporteVenta
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Estados { get; set; } = new();
        public List<ReporteVentaItem> Items { get; set; } = new();
        public decimal TotalGeneral { get; set; }
        public decimal TicketPromedio { get; set; }
        public int NumeroOperaciones { get; set; }

        public int? ClienteIdFiltro { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? EstadoFiltro { get; set; }

        public async Task OnGetAsync(int? clienteId, DateTime? fechaInicio, DateTime? fechaFin, string? estado)
        {
            ClienteIdFiltro = clienteId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            EstadoFiltro = estado;

            Clientes = await _context.Clientes
                .Select(c => new SelectListItem 
                { 
                    Value = c.Id.ToString(), 
                    Text = $"{c.Nombres} {c.Apellidos}" 
                })
                .ToListAsync();

            Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos los estados" },
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };

            await CargarDatos(clienteId, fechaInicio, fechaFin, estado);
        }

        public async Task<IActionResult> OnPostGenerarReporteAsync(int? clienteId, DateTime? fechaInicio, DateTime? fechaFin, string? estado)
        {
            await CargarDatos(clienteId, fechaInicio, fechaFin, estado);

            // Registrar el reporte generado
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario != null)
            {
                var reporte = new ReporteVentaGenerado
                {
                    FechaGeneracion = DateTime.Now,
                    UsuarioId = usuario.Id,
                    ClienteId = clienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Estado = estado,
                    ParametrosUsados = $"Cliente: {(clienteId.HasValue ? clienteId.ToString() : "Todos")}, " +
                                     $"Fecha Inicio: {(fechaInicio.HasValue ? fechaInicio.Value.ToString("dd/MM/yyyy") : "N/A")}, " +
                                     $"Fecha Fin: {(fechaFin.HasValue ? fechaFin.Value.ToString("dd/MM/yyyy") : "N/A")}, " +
                                     $"Estado: {(string.IsNullOrEmpty(estado) ? "Todos" : estado)}"
                };

                _context.ReportesVentaGenerados.Add(reporte);
                await _context.SaveChangesAsync();
            }

            ClienteIdFiltro = clienteId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            EstadoFiltro = estado;

            Clientes = await _context.Clientes
                .Select(c => new SelectListItem 
                { 
                    Value = c.Id.ToString(), 
                    Text = $"{c.Nombres} {c.Apellidos}" 
                })
                .ToListAsync();

            Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos los estados" },
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };

            return Page();
        }

        private async Task CargarDatos(int? clienteId, DateTime? fechaInicio, DateTime? fechaFin, string? estado)
        {
            var queryGuias = _context.GuiaSalida
                .Include(g => g.Cliente)
                .Include(g => g.Detalles).ThenInclude(d => d.Producto)
                .AsQueryable();

            // Aplicar filtros
            if (clienteId.HasValue)
            {
                queryGuias = queryGuias.Where(g => g.ClienteId == clienteId.Value);
            }

            if (fechaInicio.HasValue)
            {
                queryGuias = queryGuias.Where(g => g.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var fechaFinAjustada = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);
                queryGuias = queryGuias.Where(g => g.Fecha <= fechaFinAjustada);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                queryGuias = queryGuias.Where(g => g.Estado == estado);
            }

            var guias = await queryGuias.ToListAsync();

            Items = new List<ReporteVentaItem>();

            // Procesar guías de salida (ventas)
            foreach (var guia in guias)
            {
                foreach (var detalle in guia.Detalles)
                {
                    var precioUnitario = detalle.Producto?.Precio ?? 0;
                    var monto = precioUnitario * detalle.Cantidad;

                    Items.Add(new ReporteVentaItem
                    {
                        ClienteNombre = $"{guia.Cliente?.Nombres} {guia.Cliente?.Apellidos}" ?? "N/A",
                        ProductoNombre = detalle.Producto?.Nombre ?? "N/A",
                        Cantidad = detalle.Cantidad,
                        Monto = monto,
                        Fecha = guia.Fecha,
                        Vendedor = guia.Responsable,
                        Estado = guia.Estado
                    });
                }
            }

            // Calcular métricas
            NumeroOperaciones = guias.Count;
            TotalGeneral = Items.Sum(i => i.Monto);
            TicketPromedio = NumeroOperaciones > 0 ? TotalGeneral / NumeroOperaciones : 0;
        }

        public async Task<IActionResult> OnGetExportarPdfAsync(int? clienteId, DateTime? fechaInicio, DateTime? fechaFin, string? estado)
        {
            await CargarDatos(clienteId, fechaInicio, fechaFin, estado);

            QuestPDF.Settings.License = LicenseType.Community;

            var clienteNombre = "Todos los clientes";
            if (clienteId.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(clienteId.Value);
                clienteNombre = cliente != null ? $"{cliente.Nombres} {cliente.Apellidos}" : "N/A";
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("REPORTE DE VENTAS").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                            column.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                            column.Item().PaddingBottom(5);
                            column.Item().LineHorizontal(1).LineColor(Colors.Blue.Medium);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Información de filtros
                            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(col =>
                            {
                                col.Item().Text("Filtros aplicados:").Bold();
                                col.Item().Text($"Cliente: {clienteNombre}");
                                col.Item().Text($"Fecha Inicio: {(fechaInicio.HasValue ? fechaInicio.Value.ToString("dd/MM/yyyy") : "N/A")}");
                                col.Item().Text($"Fecha Fin: {(fechaFin.HasValue ? fechaFin.Value.ToString("dd/MM/yyyy") : "N/A")}");
                                col.Item().Text($"Estado: {(string.IsNullOrEmpty(estado) ? "Todos" : estado)}");
                            });

                            if (Items.Any())
                            {
                                // Métricas básicas
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Total General").Bold();
                                        header.Cell().Element(CellStyle).Text("Ticket Promedio").Bold();
                                        header.Cell().Element(CellStyle).Text("Número de Operaciones").Bold();
                                    });

                                    table.Cell().Element(CellStyle).Text($"S/ {TotalGeneral:F2}");
                                    table.Cell().Element(CellStyle).Text($"S/ {TicketPromedio:F2}");
                                    table.Cell().Element(CellStyle).Text(NumeroOperaciones.ToString());
                                });

                                // Detalle de ventas
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Cliente").Bold();
                                        header.Cell().Element(CellStyle).Text("Producto").Bold();
                                        header.Cell().Element(CellStyle).AlignCenter().Text("Cantidad").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Monto").Bold();
                                        header.Cell().Element(CellStyle).AlignCenter().Text("Fecha").Bold();
                                        header.Cell().Element(CellStyle).Text("Vendedor").Bold();
                                        header.Cell().Element(CellStyle).Text("Estado").Bold();
                                    });

                                    foreach (var item in Items)
                                    {
                                        table.Cell().Element(CellStyle).Text(item.ClienteNombre);
                                        table.Cell().Element(CellStyle).Text(item.ProductoNombre);
                                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());
                                        table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.Monto:F2}");
                                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Fecha.ToString("dd/MM/yyyy"));
                                        table.Cell().Element(CellStyle).Text(item.Vendedor);
                                        table.Cell().Element(CellStyle).Text(item.Estado);
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(7).Element(CellStyle).AlignRight().Text($"Total General: S/ {TotalGeneral:F2}").Bold();
                                    });
                                });
                            }
                            else
                            {
                                column.Item().Text("No se encontraron ventas con los filtros seleccionados.").FontColor(Colors.Grey.Medium);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"ReporteVenta_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
        }
    }

    public class ReporteVentaItem
    {
        public string ClienteNombre { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Vendedor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
