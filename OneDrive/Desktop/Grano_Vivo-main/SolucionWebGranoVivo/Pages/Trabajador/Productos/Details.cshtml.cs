using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Trabajador.Productos
{
    [Authorize(Roles = "Trabajador,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Producto Producto { get; set; } = new Producto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);

            if (Producto == null)
                return NotFound();

            return Page();
        }
    }
}
