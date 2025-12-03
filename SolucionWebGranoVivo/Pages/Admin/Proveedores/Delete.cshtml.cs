using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages_Admin_Proveedores
{
    public class DeleteModel : PageModel
    {
        private readonly SolucionWebGranoVivo.Data.ApplicationDbContext _context;

        public DeleteModel(SolucionWebGranoVivo.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Proveedor Proveedor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(m => m.Id == id);

            if (proveedor is not null)
            {
                Proveedor = proveedor;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                Proveedor = proveedor;
                _context.Proveedores.Remove(Proveedor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
