using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.PedidosCompra
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PedidoCompra PedidoCompra { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PedidoCompra = await _context.PedidosCompra
                .Include(p => p.Proveedor)
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (PedidoCompra == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var pedido = await _context.PedidosCompra
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido != null)
            {
                _context.DetallesPedidoCompra.RemoveRange(pedido.Detalles);
                _context.PedidosCompra.Remove(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}