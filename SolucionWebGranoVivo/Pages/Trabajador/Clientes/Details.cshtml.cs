using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Trabajador.Clientes
{
    [Authorize(Roles = "Trabajador,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public Cliente? Cliente { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Cliente == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
