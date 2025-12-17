using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasSalida
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<GuiaSalida> Guias { get; set; } = new();

        public async Task OnGetAsync()
        {
            Guias = await _context.GuiaSalida
                .Include(g => g.Cliente)                 
                .Include(g => g.Detalles)                
                    .ThenInclude(d => d.Producto)        
                .OrderByDescending(g => g.Fecha)
                .ToListAsync();
        }
    }
}
