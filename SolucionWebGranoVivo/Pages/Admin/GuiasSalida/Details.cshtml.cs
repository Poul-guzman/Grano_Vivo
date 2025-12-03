using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasSalida
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public GuiaSalida GuiaSalida { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            GuiaSalida = await _context.GuiaSalida
                .Include(g => g.Cliente)
                .Include(g => g.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(g => g.IdGuiaSalida == id);

            if (GuiaSalida == null)
                return NotFound();

            return Page();
        }
    }
}
