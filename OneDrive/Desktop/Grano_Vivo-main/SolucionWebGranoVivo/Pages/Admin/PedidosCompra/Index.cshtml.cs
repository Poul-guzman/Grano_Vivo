using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.PedidosCompra
{
    [Authorize(Roles = "Administrador,Trabajador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PedidoCompra> Pedidos { get; set; } = new List<PedidoCompra>();

        public async Task OnGetAsync()
        {
            Pedidos = await _context.PedidosCompra
                .Include(p => p.Proveedor)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }
    }
}
