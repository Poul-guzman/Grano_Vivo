using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Cotizacion Cotizacion { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Cotizacion = await _context.Cotizaciones
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Cotizacion == null)
                return NotFound();

            return Page();
        }
    }
}
