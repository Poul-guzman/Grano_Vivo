using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SolucionWebGranoVivo.Pages.Admin.ReporteCompra
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

        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<ReporteCompraItem> Items { get; set; } = new();
        public Dictionary<int, decimal> TotalesPorProveedor { get; set; } = new();
        public Dictionary<int, decimal> CostoPromedioPorProducto { get; set; } = new();
        public decimal TotalGeneral { get; set; }

        public int? ProveedorIdFiltro { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public async Task OnGetAsync(int? proveedorId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ProveedorIdFiltro = proveedorId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;

            Proveedores = await _context.Proveedores
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToListAsync();

            await CargarDatos(proveedorId, fechaInicio, fechaFin);
        }

        public async Task<IActionResult> OnPostGenerarReporteAsync(int? proveedorId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ProveedorIdFiltro = proveedorId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;

            Proveedores = await _context.Proveedores
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToListAsync();

            await CargarDatos(proveedorId, fechaInicio, fechaFin);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario != null)
            {
                var reporte = new ReporteCompraGenerado
                {
                    FechaGeneracion = DateTime.Now,
                    UsuarioId = usuario.Id,
                    ProveedorId = proveedorId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    ParametrosUsados = $"Proveedor: {(proveedorId.HasValue ? proveedorId.ToString() : "Todos")}, " +
                                     $"Fecha Inicio: {(fechaInicio.HasValue ? fechaInicio.Value.ToString("dd/MM/yyyy") : "N/A")}, " +
                                     $"Fecha Fin: {(fechaFin.HasValue ? fechaFin.Value.ToString("dd/MM/yyyy") : "N/A")}"
                };

                _context.ReportesCompraGenerados.Add(reporte);
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        private async Task CargarDatos(int? proveedorId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var queryOrdenes = _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles).ThenInclude(d => d.Producto)
                .AsQueryable();

            var queryPedidos = _context.PedidosCompra
                .Include(p => p.Proveedor)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .AsQueryable();

            if (proveedorId.HasValue)
            {
                queryOrdenes = queryOrdenes.Where(o => o.ProveedorId == proveedorId.Value);
                queryPedidos = queryPedidos.Where(p => p.ProveedorId == proveedorId.Value);
            }

            if (fechaInicio.HasValue)
            {
                queryOrdenes = queryOrdenes.Where(o => o.Fecha >= fechaInicio.Value);
                queryPedidos = queryPedidos.Where(p => p.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var fechaFinAjustada = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);
                queryOrdenes = queryOrdenes.Where(o => o.Fecha <= fechaFinAjustada);
                queryPedidos = queryPedidos.Where(p => p.Fecha <= fechaFinAjustada);
            }

            var ordenes = await queryOrdenes.ToListAsync();
            var pedidos = await queryPedidos.ToListAsync();

            Items = new List<ReporteCompraItem>();

            foreach (var orden in ordenes)
            {
                foreach (var detalle in orden.Detalles)
                {
                    Items.Add(new ReporteCompraItem
                    {
                        ProductoNombre = detalle.Producto?.Nombre ?? "N/A",
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Monto = detalle.SubTotal,
                        Fecha = orden.Fecha,
                        ProveedorNombre = orden.Proveedor?.Nombre ?? "N/A",
                        ProveedorId = orden.ProveedorId
                    });
                }
            }

            foreach (var pedido in pedidos)
            {
                foreach (var detalle in pedido.Detalles)
                {
                    Items.Add(new ReporteCompraItem
                    {
                        ProductoNombre = detalle.Producto?.Nombre ?? "N/A",
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Monto = detalle.SubTotal,
                        Fecha = pedido.Fecha,
                        ProveedorNombre = pedido.Proveedor?.Nombre ?? "N/A",
                        ProveedorId = pedido.ProveedorId
                    });
                }
            }

            TotalesPorProveedor = Items
                .GroupBy(i => i.ProveedorId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Monto));

            var promedios = Items
                .GroupBy(i => i.ProductoNombre)
                .ToDictionary(g => g.Key.GetHashCode(), g => g.Average(i => i.PrecioUnitario));
            CostoPromedioPorProducto = promedios;

            TotalGeneral = Items.Sum(i => i.Monto);
        }

        public async Task<IActionResult> OnGetExportarPdfAsync(int? proveedorId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            await CargarDatos(proveedorId, fechaInicio, fechaFin);

            QuestPDF.Settings.License = LicenseType.Community;

            var proveedorNombre = "Todos los proveedores";
            if (proveedorId.HasValue)
            {
                var proveedor = await _context.Proveedores.FindAsync(proveedorId.Value);
                proveedorNombre = proveedor?.Nombre ?? "N/A";
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
                            column.Item().Text("REPORTE DE COMPRAS").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                            column.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                            column.Item().PaddingBottom(5);
                            column.Item().LineHorizontal(1).LineColor(Colors.Blue.Medium);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(col =>
                            {
                                col.Item().Text("Filtros aplicados:").Bold();
                                col.Item().Text($"Proveedor: {proveedorNombre}");
                                col.Item().Text($"Fecha Inicio: {(fechaInicio.HasValue ? fechaInicio.Value.ToString("dd/MM/yyyy") : "N/A")}");
                                col.Item().Text($"Fecha Fin: {(fechaFin.HasValue ? fechaFin.Value.ToString("dd/MM/yyyy") : "N/A")}");
                            });

                            if (Items.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Proveedor").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Total (S/)").Bold();
                                    });

                                    foreach (var total in TotalesPorProveedor)
                                    {
                                        var nombre = Items.FirstOrDefault(i => i.ProveedorId == total.Key)?.ProveedorNombre ?? "N/A";
                                        table.Cell().Element(CellStyle).Text(nombre);
                                        table.Cell().Element(CellStyle).AlignRight().Text($"S/ {total.Value:F2}");
                                    }
                                });

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Producto").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Costo Promedio (S/)").Bold();
                                    });

                                    foreach (var producto in Items.GroupBy(i => i.ProductoNombre))
                                    {
                                        var promedio = producto.Average(i => i.PrecioUnitario);
                                        table.Cell().Element(CellStyle).Text(producto.Key);
                                        table.Cell().Element(CellStyle).AlignRight().Text($"S/ {promedio:F2}");
                                    }
                                });

                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Producto").Bold();
                                        header.Cell().Element(CellStyle).AlignCenter().Text("Cantidad").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.").Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text("Monto").Bold();
                                        header.Cell().Element(CellStyle).AlignCenter().Text("Fecha").Bold();
                                        header.Cell().Element(CellStyle).Text("Proveedor").Bold();
                                    });

                                    foreach (var item in Items)
                                    {
                                        table.Cell().Element(CellStyle).Text(item.ProductoNombre);
                                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());
                                        table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.PrecioUnitario:F2}");
                                        table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.Monto:F2}");
                                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Fecha.ToString("dd/MM/yyyy"));
                                        table.Cell().Element(CellStyle).Text(item.ProveedorNombre);
                                    }

                                    table.Footer(footer =>
                                    {
                                        footer.Cell().ColumnSpan(6).Element(CellStyle).AlignRight().Text($"Total General: S/ {TotalGeneral:F2}").Bold();
                                    });
                                });
                            }
                            else
                            {
                                column.Item().Text("No se encontraron compras con los filtros seleccionados.").FontColor(Colors.Grey.Medium);
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

            return File(stream, "application/pdf", $"ReporteCompra_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
        }
    }

    public class ReporteCompraItem
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public int ProveedorId { get; set; }
    }
}
