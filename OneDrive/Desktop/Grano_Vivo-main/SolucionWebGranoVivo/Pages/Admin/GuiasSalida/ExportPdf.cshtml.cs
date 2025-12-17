using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.IO;
using System.Linq;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasSalida
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

        public IActionResult OnGet(string id)
        {
            var guia =  _context.GuiaSalida
                .Include(g => g.Cliente)
                .Include(g => g.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefault(g => g.IdGuiaSalida == id);

            if (guia == null) return NotFound();

            var logoPath = Path.Combine(_env.WebRootPath, "img", "logocafe.jpg");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    
                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(60).Image(logoPath, ImageScaling.FitWidth);
                        row.RelativeColumn().AlignCenter().Column(col =>
                        {
                            col.Item().Text("GRANO VIVO SELECTO").FontSize(18).Bold();
                        });
                        row.ConstantColumn(130).Border(1).Padding(5).AlignCenter().Column(col =>
                        {
                            col.Item().Text("GUIA SALIDA").FontSize(10).Bold();
                            col.Item().Text(guia.IdGuiaSalida).FontSize(12).Bold();
                        });

                    });

                    
                    page.Content().Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"Fecha: {guia.Fecha:dd/MM/yyyy}");
                        col.Item().Text($"Cliente: {guia.Cliente?.Nombres} {guia.Cliente?.Apellidos}");
                        col.Item().Text($"Responsable: {guia.Responsable}");
                        col.Item().Text($"Estado: {guia.Estado}");
                        col.Item().Text($"Observaciones: {guia.Observaciones}");

                        col.Item().LineHorizontal(1);

                        
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(1); 
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Producto").Bold();
                                header.Cell().AlignRight().Text("Cantidad").Bold();
                            });

                            foreach (var d in guia.Detalles)
                            {
                                table.Cell().Text(d.Producto?.Nombre ?? "-");
                                table.Cell().AlignRight().Text(d.Cantidad.ToString());
                            }
                        });
                    });

                   
                    page.Footer().AlignCenter().Text("Generado automáticamente por Grano Vivo Selecto");
                });
            });

            
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"Guia_{guia.IdGuiaSalida}.pdf");
        }
    }
}
