using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;


namespace SolucionWebGranoVivo.Pages.Admin.GuiasEntrada
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;


        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public GuiaEntrada GuiaEntrada { get; set; }


        public IList<Producto> Productos { get; set; }
        public IList<Proveedor> Proveedores { get; set; }
        public SelectList ProveedoresSelectList { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Productos = await _context.Productos.ToListAsync();
            Proveedores = await _context.Proveedores.ToListAsync();
            ProveedoresSelectList = new SelectList(Proveedores, "Id", "Nombre");
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Productos = await _context.Productos.ToListAsync();
                Proveedores = await _context.Proveedores.ToListAsync();
                return Page();
            }


            _context.GuiasEntrada.Add(GuiaEntrada);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}