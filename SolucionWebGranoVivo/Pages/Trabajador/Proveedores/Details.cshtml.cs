using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Trabajador.Proveedores
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Proveedor Proveedor { get; set; } = new Proveedor();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id);

            if (Proveedor == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
