using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Trabajador.Proveedores
{
    [Authorize(Roles = "Trabajador,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Proveedor> Proveedor { get; set; } = new List<Proveedor>();

        public async Task OnGetAsync()
        {
            Proveedor = await _context.Proveedores.ToListAsync();
        }
    }
}
