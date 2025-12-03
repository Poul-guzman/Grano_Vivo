using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Clientes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            _context.Clientes.Add(Cliente);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
