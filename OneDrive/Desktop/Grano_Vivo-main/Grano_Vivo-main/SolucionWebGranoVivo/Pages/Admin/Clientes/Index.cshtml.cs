using Microsoft.AspNetCore.Mvc.RazorPages;
using SolucionWebGranoVivo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;

namespace SolucionWebGranoVivo.Pages.Admin.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cliente> Cliente { get; set; }

        public async Task OnGetAsync()
        {
            Cliente = await _context.Clientes.ToListAsync();
        }
    }
}
