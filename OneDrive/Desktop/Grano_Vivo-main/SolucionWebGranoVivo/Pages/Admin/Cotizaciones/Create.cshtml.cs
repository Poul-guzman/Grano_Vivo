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

namespace SolucionWebGranoVivo.Pages.Admin.Cotizaciones
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
        public Cotizacion Cotizacion { get; set; } = new();

        [BindProperty]
        public List<DetalleCotizacion> DetallesCotizacion { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();

        
        public List<Producto> Productos { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync invoked. Form keys: {Keys}", string.Join(", ", Request.Form.Keys));
            _logger.LogInformation("Cotizacion.ClienteId: {ClienteId}", Cotizacion?.ProveedorId);
            _logger.LogInformation("DetallesCotizacion count (before normalization): {Count}", DetallesCotizacion?.Count ?? 0);

           
            if (DetallesCotizacion == null)
            {
                DetallesCotizacion = new List<DetalleCotizacion>();
            }

            
            if (!DetallesCotizacion.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto a la cotización.");
            }

            
            foreach (var d in DetallesCotizacion)
            {
                if (d.Cantidad < 0) d.Cantidad = 0;
                if (d.PrecioUnitario < 0) d.PrecioUnitario = 0m;

                d.SubTotal = d.PrecioUnitario * d.Cantidad;

                _logger.LogDebug("Detalle - ProductoId: {Pid}, Cant: {Cant}, Precio: {Precio}, Sub: {Sub}",
                    d.ProductoId, d.Cantidad, d.PrecioUnitario, d.SubTotal);
            }

            Cotizacion.Total = DetallesCotizacion.Sum(d => d.SubTotal);
            _logger.LogInformation("Cotizacion.Total calculated: {Total}", Cotizacion.Total);

          
            var lastCodigo = await _context.Cotizaciones
                .Where(c => c.Codigo.StartsWith("CO"))
                .OrderByDescending(c => c.Codigo)
                .Select(c => c.Codigo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 2)
            {
                var numericPart = lastCodigo.Substring(2);
                if (int.TryParse(numericPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            Cotizacion.Codigo = $"CO{nextNumber:000}";
            _logger.LogInformation("Generated Codigo for cotizacion: {Codigo}", Cotizacion.Codigo);

           
            ModelState.Remove("Cotizacion.Codigo");
   

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido al intentar crear cotización. Errores: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                await LoadSelectListsAsync();
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Cotizaciones.Add(Cotizacion);
                await _context.SaveChangesAsync(); 

                foreach (var d in DetallesCotizacion)
                {
                    d.CotizacionId = Cotizacion.Id;
                    _context.DetallesCotizacion.Add(d);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Cotizacion creada correctamente. Id: {Id}, Codigo: {Codigo}", Cotizacion.Id, Cotizacion.Codigo);
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "DbUpdateException al guardar cotización. Codigo: {Codigo}", Cotizacion.Codigo);

                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la cotización. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Excepción inesperada al guardar cotización.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intente nuevamente.");
            }

        
            await LoadSelectListsAsync();
            return Page();
        }

       
        private async Task LoadSelectListsAsync()
        {
            ProveedoresSelectList = await _context.Proveedores
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Nombre}"
                })
                .ToListAsync();

            Productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}
