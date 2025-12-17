using Microsoft.AspNetCore.Mvc.RazorPages;
using SolucionWebGranoVivo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;

namespace SolucionWebGranoVivo.Pages.Admin.Proveedores
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Proveedor> Proveedor { get; set; }

        public async Task OnGetAsync()
        {
            Proveedor = await _context.Proveedores.ToListAsync();
        }
    }
}
