using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.PedidosCompra
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PedidoCompra PedidoCompra { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PedidoCompra = await _context.PedidosCompra
                .Include(p => p.Proveedor)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (PedidoCompra == null)
                return NotFound();

            return Page();
        }
    }
}
