using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;


namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<GuiaEntrada> GuiasEntrada { get; set; } = new List<GuiaEntrada>();


        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task OnGetAsync()
        {
            GuiasEntrada = await _context.GuiasEntrada
            .Include(g => g.Proveedor)
            .ToListAsync();
        }
    }
}