using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.OrdenesCompra
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrdenCompra OrdenCompra { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrdenCompra = await _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (OrdenCompra == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var orden = await _context.OrdenesCompra
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden != null)
            {
                _context.DetallesOrdenCompra.RemoveRange(orden.Detalles);
                _context.OrdenesCompra.Remove(orden);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
