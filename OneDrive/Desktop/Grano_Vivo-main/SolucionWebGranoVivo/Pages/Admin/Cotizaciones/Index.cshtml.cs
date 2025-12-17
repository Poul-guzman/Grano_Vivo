using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
{
    [Authorize(Roles = "Administrador,Trabajador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();

        public async Task OnGetAsync()
        {
            Cotizaciones = await _context.Cotizaciones
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }
    }
}
