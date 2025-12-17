using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Proveedores
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Proveedor Proveedor { get; set; } = default!;

        public IActionResult OnGet()
        {
            
            var ultimoCodigo = _context.Proveedores
                .OrderByDescending(p => p.Id)
                .Select(p => p.Codigo)
                .FirstOrDefault();

            int nuevoNumero = 1;

            if (!string.IsNullOrEmpty(ultimoCodigo))
            {
                string numeroStr = ultimoCodigo.Substring(3); 
                if (int.TryParse(numeroStr, out int numero))
                {
                    nuevoNumero = numero + 1;
                }
            }

            
            Proveedor = new Proveedor
            {
                Codigo = $"PRV{nuevoNumero:D3}"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            
            var ultimoCodigo = _context.Proveedores
                .OrderByDescending(p => p.Id)
                .Select(p => p.Codigo)
                .FirstOrDefault();

            int nuevoNumero = 1;

            if (!string.IsNullOrEmpty(ultimoCodigo))
            {
                string numeroStr = ultimoCodigo.Substring(3);
                if (int.TryParse(numeroStr, out int numero))
                {
                    nuevoNumero = numero + 1;
                }
            }

            Proveedor.Codigo = $"PRV{nuevoNumero:D3}";

            _context.Proveedores.Add(Proveedor);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
