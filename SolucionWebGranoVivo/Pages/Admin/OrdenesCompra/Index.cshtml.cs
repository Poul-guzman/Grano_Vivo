using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.OrdenesCompra
{
    [Authorize(Roles = "Administrador,Trabajador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<OrdenCompra> Ordenes { get; set; } = new List<OrdenCompra>();

        public async Task OnGetAsync()
        {
            Ordenes = await _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(o => o.Fecha)
                .ToListAsync();
        }
    }
}
