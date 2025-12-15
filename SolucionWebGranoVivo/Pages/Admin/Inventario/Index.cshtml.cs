using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using SolucionWebGranoVivo.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SolucionWebGranoVivo.Pages.Admin.Inventario
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly InventoryGeneratorService _service;

        public IndexModel(ApplicationDbContext context, InventoryGeneratorService service)
        {
            _context = context;
            _service = service;
        }

        public List<Producto> Productos { get; set; } = new();
        public string? ArchivoGenerado { get; private set; }

        public async Task OnGetAsync()
        {
            Productos = await _context.Productos.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ArchivoGenerado = await _service.GenerateAsync();
            var bytes = System.IO.File.ReadAllBytes(ArchivoGenerado);
            return File(bytes, "text/csv", Path.GetFileName(ArchivoGenerado));
        }
    }
}
