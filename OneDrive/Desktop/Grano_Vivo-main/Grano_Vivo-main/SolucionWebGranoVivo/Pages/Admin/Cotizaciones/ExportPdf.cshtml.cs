using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
{
    public class ExportPdfModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExportPdfModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cotizacion == null) return NotFound();

            var logoPath = Path.Combine(_env.WebRootPath, "img", "logocafe.jpg");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    
                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(60).Image(logoPath, ImageScaling.FitWidth);
                        row.RelativeColumn().AlignCenter().Column(col =>
                        {
                            col.Item().Text("GRANO VIVO SELECTO").FontSize(18).Bold();
                        });
                        row.ConstantColumn(130).Border(1).Padding(5).AlignCenter().Column(col =>
                        {
                            col.Item().Text("COTIZACIÓN").FontSize(10).Bold();
                            col.Item().Text(cotizacion.Codigo).FontSize(12).Bold();
                        });
                    });


                    page.Content().Column(col =>
                    {
                        col.Spacing(6);

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().Text($"Fecha: {cotizacion.Fecha:dd/MM/yyyy}");
                                c.Item().Text($"Cliente: {cotizacion.Proveedor?.Nombre}");
                                c.Item().Text($"Estado: {cotizacion.Estado}");
                            });
                            
                        });

                        col.Item().LineHorizontal(1);

                        
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(5); 
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(2); 
                                columns.RelativeColumn(2); 
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Producto").Bold();
                                header.Cell().AlignRight().Text("Cant.").Bold();
                                header.Cell().AlignRight().Text("P. Unitario").Bold();
                                header.Cell().AlignRight().Text("Subtotal").Bold();
                            });

                            foreach (var d in cotizacion.Detalles)
                            {
                                table.Cell().Text(d.Producto?.Nombre ?? "-");
                                table.Cell().AlignRight().Text(d.Cantidad.ToString());
                                table.Cell().AlignRight().Text(d.PrecioUnitario.ToString("C"));
                                table.Cell().AlignRight().Text(d.SubTotal.ToString("C"));
                            }

                            
                        });

                        col.Item().LineHorizontal(1);

                        col.Item().AlignRight().Text($"Total: {cotizacion.Total.ToString("C", CultureInfo.CreateSpecificCulture("es-PE"))}").FontSize(12).Bold();
                    });

                    
                    page.Footer().AlignCenter().Text("Generado automáticamente por Grano Vivo Selecto");
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            var fileName = $"Cotizacion_{cotizacion.Codigo}.pdf";
            return File(stream.ToArray(), "application/pdf", fileName);
        }
    }
}
