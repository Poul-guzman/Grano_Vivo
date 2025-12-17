using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Pages.Admin.PedidosCompra
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
        public PedidoCompra PedidoCompra { get; set; } = new();

        [BindProperty]
        public List<DetallePedidoCompra> DetallesPedidoCompra { get; set; } = new();

        public List<SelectListItem> ProveedoresSelectList { get; set; } = new();
        public List<Producto> Productos { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (DetallesPedidoCompra == null)
                DetallesPedidoCompra = new List<DetallePedidoCompra>();

            if (!DetallesPedidoCompra.Any())
                ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto al pedido.");

            foreach (var d in DetallesPedidoCompra)
            {
                if (d.Cantidad < 0) d.Cantidad = 0;
                if (d.PrecioUnitario < 0) d.PrecioUnitario = 0m;
                d.SubTotal = d.Cantidad * d.PrecioUnitario;
            }

            PedidoCompra.Total = DetallesPedidoCompra.Sum(d => d.SubTotal);

            
            var lastCodigo = await _context.PedidosCompra
                .Where(p => p.Codigo.StartsWith("PE"))
                .OrderByDescending(p => p.Codigo)
                .Select(p => p.Codigo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 2)
            {
                var numericPart = lastCodigo.Substring(2);
                if (int.TryParse(numericPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            PedidoCompra.Codigo = $"PE{nextNumber:000}";
            ModelState.Remove("PedidoCompra.Codigo");

            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.PedidosCompra.Add(PedidoCompra);
                await _context.SaveChangesAsync();

                foreach (var d in DetallesPedidoCompra)
                {
                    d.PedidoCompraId = PedidoCompra.Id;
                    _context.DetallesPedidoCompra.Add(d);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al guardar PedidoCompra");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el pedido.");
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
