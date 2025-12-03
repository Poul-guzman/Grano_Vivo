using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cotizacion Cotizacion { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Cotizacion = await _context.Cotizaciones
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Cotizacion == null)
                return NotFound();

            ProveedoresSelectList = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Nombres} {c.Apellidos}"
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                
                ProveedoresSelectList = _context.Clientes
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.Nombres} {c.Apellidos}"
                    }).ToList();

                return Page();
            }

           
            var cotizacionDb = await _context.Cotizaciones
                .Include(c => c.Detalles) 
                .FirstOrDefaultAsync(c => c.Id == Cotizacion.Id);

            if (cotizacionDb == null)
                return NotFound();

            
            cotizacionDb.ProveedorId = Cotizacion.ProveedorId;
            cotizacionDb.Estado = Cotizacion.Estado;
            
            cotizacionDb.Total = Cotizacion.Total;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Cotizaciones.AnyAsync(e => e.Id == Cotizacion.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("Index");
        }
    }
}
