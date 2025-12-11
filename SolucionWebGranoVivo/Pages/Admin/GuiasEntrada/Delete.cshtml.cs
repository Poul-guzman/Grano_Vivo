using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;


namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public GuiaEntrada GuiaEntrada { get; set; }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            GuiaEntrada = await _context.GuiasEntrada
                .Include(g => g.Proveedor)
                .FirstOrDefaultAsync(m => m.IdGuiaEntrada == id);


            if (GuiaEntrada == null)
            {
                return NotFound();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string id)
        {
            var guia = await _context.GuiasEntrada.FindAsync(id);
            if (guia != null)
            {
                _context.GuiasEntrada.Remove(guia);
                await _context.SaveChangesAsync();
            }


            return RedirectToPage("Index");
        }
    }
}