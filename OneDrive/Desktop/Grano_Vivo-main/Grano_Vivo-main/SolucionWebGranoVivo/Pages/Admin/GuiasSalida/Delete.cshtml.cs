using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasSalida
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuiaSalida GuiaSalida { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            GuiaSalida = await _context.GuiaSalida
                .Include(g => g.Cliente)
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.IdGuiaSalida == id);

            if (GuiaSalida == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var guia = await _context.GuiaSalida
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.IdGuiaSalida == id);

            if (guia != null)
            {
                _context.DetalleGuiaSalida.RemoveRange(guia.Detalles);
                _context.GuiaSalida.Remove(guia);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
