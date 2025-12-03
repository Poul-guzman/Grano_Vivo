using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.PedidosCompra
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PedidoCompra PedidoCompra { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PedidoCompra = await _context.PedidosCompra
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (PedidoCompra == null)
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

            var pedidoDb = await _context.PedidosCompra
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == PedidoCompra.Id);

            if (pedidoDb == null)
                return NotFound();

            pedidoDb.ProveedorId = PedidoCompra.ProveedorId;
            pedidoDb.Estado = PedidoCompra.Estado;
            pedidoDb.Total = PedidoCompra.Total;

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
