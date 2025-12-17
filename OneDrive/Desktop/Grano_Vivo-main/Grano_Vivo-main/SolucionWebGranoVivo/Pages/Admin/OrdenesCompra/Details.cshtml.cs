using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.OrdenesCompra
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public OrdenCompra OrdenCompra { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrdenCompra = await _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (OrdenCompra == null)
                return NotFound();

            return Page();
        }
    }
}
