using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cotizacion Cotizacion { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Cotizacion = await _context.Cotizaciones
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Cotizacion == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cotizacion != null)
            {
                _context.DetallesCotizacion.RemoveRange(cotizacion.Detalles);
                _context.Cotizaciones.Remove(cotizacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
