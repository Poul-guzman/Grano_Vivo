using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Productos 
{
    [Authorize(Roles = "Administrador")] 
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Producto> Producto { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Producto = await _context.Productos?.ToListAsync() ?? new List<Producto>();
        }
    }
}
