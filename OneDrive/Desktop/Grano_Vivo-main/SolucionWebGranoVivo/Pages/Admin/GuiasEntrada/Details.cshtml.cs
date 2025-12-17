using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;


namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;


        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }


        public GuiaEntrada GuiaEntrada { get; set; }


        public async Task OnGetAsync(string idGuiaEntrada)
        {
            GuiaEntrada = await _context.GuiasEntrada
                .Include(g => g.Proveedor)
                .Include(g => g.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(g => g.IdGuiaEntrada == idGuiaEntrada);
        }
    }
}