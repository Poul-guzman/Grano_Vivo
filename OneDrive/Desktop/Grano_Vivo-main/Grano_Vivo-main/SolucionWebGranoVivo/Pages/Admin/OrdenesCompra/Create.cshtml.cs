using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.OrdenesCompra
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public OrdenCompra OrdenCompra { get; set; } = new();

        [BindProperty]
        public List<DetalleOrdenCompra> DetallesOrdenCompra { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();
        public List<Producto> Productos { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync invoked for OrdenCompra. Keys: {Keys}", string.Join(", ", Request.Form.Keys));

            if (DetallesOrdenCompra == null)
                DetallesOrdenCompra = new List<DetalleOrdenCompra>();

            if (!DetallesOrdenCompra.Any())
                ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto a la orden de compra.");

            foreach (var d in DetallesOrdenCompra)
            {
                if (d.Cantidad < 0) d.Cantidad = 0;
                if (d.PrecioUnitario < 0) d.PrecioUnitario = 0m;
                d.SubTotal = d.PrecioUnitario * d.Cantidad;
            }

            OrdenCompra.Total = DetallesOrdenCompra.Sum(d => d.SubTotal);

            
            var lastCodigo = await _context.OrdenesCompra
                .Where(o => o.Codigo.StartsWith("OC"))
                .OrderByDescending(o => o.Codigo)
                .Select(o => o.Codigo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 2)
            {
                var numericPart = lastCodigo.Substring(2);
                if (int.TryParse(numericPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            OrdenCompra.Codigo = $"OC{nextNumber:000}";
            ModelState.Remove("OrdenCompra.Codigo");

            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.OrdenesCompra.Add(OrdenCompra);
                await _context.SaveChangesAsync();

                foreach (var d in DetallesOrdenCompra)
                {
                    d.OrdenCompraId = OrdenCompra.Id;
                    _context.DetallesOrdenCompra.Add(d);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al guardar OrdenCompra");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la orden de compra.");
            }

            await LoadSelectListsAsync();
            return Page();
        }

        private async Task LoadSelectListsAsync()
        {
            ProveedoresSelectList = await _context.Proveedores
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();

            Productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}
