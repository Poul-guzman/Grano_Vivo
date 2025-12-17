using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using SolucionWebGranoVivo.Services;
using System.Linq;
using System;

namespace SolucionWebGranoVivo.Pages
{
    [IgnoreAntiforgeryToken]
    public class TiendaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TiendaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Producto> Productos { get; set; } = new List<Producto>();
        public List<CarritoItem> Carrito { get; set; } = new List<CarritoItem>();
        public int CantidadCarrito { get; set; }

        public async Task OnGetAsync()
        {
            Carrito = CarritoService.ObtenerCarrito(HttpContext.Session);
            CantidadCarrito = CarritoService.ObtenerCantidadTotal(HttpContext.Session);
        }

        public async Task<IActionResult> OnPostAgregarAlCarritoAsync(int productoId, string productoNombre, decimal productoPrecio, string productoImagen, int productoStock, int cantidad = 1)
        {
            try
            {
                
                var producto = new Producto
                {
                    Id = productoId,
                    Nombre = productoNombre ?? string.Empty,
                    Precio = productoPrecio,
                    ImagenUrl = productoImagen ?? string.Empty,
                    Stock = productoStock
                };

            if (producto.Stock < cantidad)
            {
                return new JsonResult(new { success = false, message = $"Solo hay {producto.Stock} unidades disponibles." });
            }

            var carrito = CarritoService.ObtenerCarrito(HttpContext.Session);
            var itemExistente = carrito.FirstOrDefault(x => x.ProductoId == productoId);
            
            if (itemExistente != null)
            {
                if (itemExistente.Cantidad + cantidad > producto.Stock)
                {
                    return new JsonResult(new { success = false, message = $"No hay suficiente stock. Disponible: {producto.Stock} unidades." });
                }
            }

                CarritoService.AgregarProducto(HttpContext.Session, producto, cantidad);
                
                var nuevaCantidad = CarritoService.ObtenerCantidadTotal(HttpContext.Session);
                
                return new JsonResult(new { success = true, cantidad = nuevaCantidad, message = "Producto agregado al carrito." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult OnPostActualizarCantidad(int productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                return new JsonResult(new { success = false, message = "La cantidad debe ser mayor a 0." });
            }

            var carrito = CarritoService.ObtenerCarrito(HttpContext.Session);
            var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);

            if (item == null)
            {
                return new JsonResult(new { success = false, message = "Producto no encontrado en el carrito." });
            }

            if (cantidad > item.StockDisponible)
            {
                return new JsonResult(new { success = false, message = $"Solo hay {item.StockDisponible} unidades disponibles." });
            }

            CarritoService.ActualizarCantidad(HttpContext.Session, productoId, cantidad);
            
            carrito = CarritoService.ObtenerCarrito(HttpContext.Session);
            var subtotal = item.Precio * cantidad;
            var totalCarrito = CarritoService.CalcularSubtotal(HttpContext.Session);
            var impuesto = CarritoService.CalcularImpuesto(HttpContext.Session);
            var total = CarritoService.CalcularTotal(HttpContext.Session);

            return new JsonResult(new { 
                success = true, 
                subtotal = subtotal.ToString("F2"),
                totalCarrito = totalCarrito.ToString("F2"),
                impuesto = impuesto.ToString("F2"),
                total = total.ToString("F2")
            });
        }

        public IActionResult OnPostEliminarDelCarrito(int productoId)
        {
            CarritoService.EliminarProducto(HttpContext.Session, productoId);
            
            var cantidad = CarritoService.ObtenerCantidadTotal(HttpContext.Session);
            var totalCarrito = CarritoService.CalcularSubtotal(HttpContext.Session);
            var impuesto = CarritoService.CalcularImpuesto(HttpContext.Session);
            var total = CarritoService.CalcularTotal(HttpContext.Session);

            return new JsonResult(new { 
                success = true, 
                cantidad = cantidad,
                totalCarrito = totalCarrito.ToString("F2"),
                impuesto = impuesto.ToString("F2"),
                total = total.ToString("F2")
            });
        }

        public async Task<IActionResult> OnPostConfirmarPedidoAsync(string nombreCliente, string emailCliente, string telefonoCliente, string direccionCliente)
        {
            var carrito = CarritoService.ObtenerCarrito(HttpContext.Session);
            
            if (carrito == null || !carrito.Any())
            {
                return new JsonResult(new { success = false, message = "El carrito está vacío." });
            }

            if (string.IsNullOrWhiteSpace(nombreCliente) || 
                string.IsNullOrWhiteSpace(emailCliente) || 
                string.IsNullOrWhiteSpace(telefonoCliente) || 
                string.IsNullOrWhiteSpace(direccionCliente))
            {
                return new JsonResult(new { success = false, message = "Por favor complete todos los datos obligatorios." });
            }

        
            var erroresStock = new List<string>();
            foreach (var item in carrito)
            {
                
                if (item.Cantidad > item.StockDisponible)
                {
                    erroresStock.Add($"No hay suficiente stock de {item.Nombre}. Disponible: {item.StockDisponible}, Solicitado: {item.Cantidad}.");
                }
            }

            if (erroresStock.Any())
            {
                return new JsonResult(new { success = false, message = string.Join(" ", erroresStock) });
            }

            CarritoService.LimpiarCarrito(HttpContext.Session);

            return new JsonResult(new { 
                success = true, 
                message = $"¡Pedido confirmado exitosamente! Se enviará un correo de confirmación a {emailCliente}." 
            });
        }
    }
}
