using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Productos
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Producto { get; set; } = default!;

        public IActionResult OnGet()
        {
           
            var ultimoCodigo = _context.Productos
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

            
            Producto = new Producto
            {
                Codigo = $"PRD{nuevoNumero:D3}"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            var ultimoCodigo = _context.Productos
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

            Producto.Codigo = $"PRD{nuevoNumero:D3}";

            _context.Productos.Add(Producto);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
