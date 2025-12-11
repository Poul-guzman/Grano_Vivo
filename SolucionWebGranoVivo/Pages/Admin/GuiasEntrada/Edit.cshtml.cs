using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuiaEntrada GuiaEntrada { get; set; }

        public IList<Producto> Productos { get; set; }
        public SelectList ProveedoresSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            GuiaEntrada = await _context.GuiasEntrada
            .Include(g => g.Detalles)
            .FirstOrDefaultAsync(m => m.IdGuiaEntrada == id);

            Productos = await _context.Productos.ToListAsync();

            var proveedores = await _context.Proveedores.ToListAsync();
            ProveedoresSelectList = new SelectList(proveedores, "Id", "Nombre");

            if (GuiaEntrada == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Productos = await _context.Productos.ToListAsync();
                var proveedores = await _context.Proveedores.ToListAsync();
                ProveedoresSelectList = new SelectList(proveedores, "Id", "Nombre");
                return Page();
            }

            _context.Attach(GuiaEntrada).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}