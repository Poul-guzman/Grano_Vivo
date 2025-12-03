using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.OrdenesCompra
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrdenCompra OrdenCompra { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OrdenCompra = await _context.OrdenesCompra
                .Include(o => o.Proveedor)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (OrdenCompra == null)
                return NotFound();

            ProveedoresSelectList = _context.Proveedores
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ProveedoresSelectList = _context.Proveedores
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    }).ToList();
                return Page();
            }

            var ordenDb = await _context.OrdenesCompra
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == OrdenCompra.Id);

            if (ordenDb == null)
                return NotFound();

            ordenDb.ProveedorId = OrdenCompra.ProveedorId;
            ordenDb.Estado = OrdenCompra.Estado;
            ordenDb.Total = OrdenCompra.Total;

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
