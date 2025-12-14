using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuiaEntrada GuiaEntrada { get; set; } = new();

        public List<Proveedor> Proveedores { get; set; } = new();
        public List<Producto> Productos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Proveedores = await _context.Proveedores.ToListAsync();
            Productos = await _context.Productos.ToListAsync();

            // 👇 IMPORTANTE: inicializar al menos una fila
            GuiaEntrada.Detalles.Add(new DetalleGuiaEntrada());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Proveedores = await _context.Proveedores.ToListAsync();
                Productos = await _context.Productos.ToListAsync();
                return Page();
            }

            _context.GuiasEntrada.Add(GuiaEntrada);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
