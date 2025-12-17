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
    public class DetailsModel : PageModel
    {
        private readonly SolucionWebGranoVivo.Data.ApplicationDbContext _context;

        public DetailsModel(SolucionWebGranoVivo.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
