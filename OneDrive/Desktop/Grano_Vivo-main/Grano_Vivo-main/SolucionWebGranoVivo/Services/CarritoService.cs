using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SolucionWebGranoVivo.Models;

namespace SolucionWebGranoVivo.Services
{
    public class CarritoService
    {
        private const string CarritoSessionKey = "CarritoCompras";

        public static List<CarritoItem> ObtenerCarrito(ISession session)
        {
            var carritoJson = session.GetString(CarritoSessionKey);
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItem>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson) ?? new List<CarritoItem>();
            }
            catch
            {
                return new List<CarritoItem>();
            }
        }

        public static void GuardarCarrito(ISession session, List<CarritoItem> carrito)
        {
            var carritoJson = JsonSerializer.Serialize(carrito);
            session.SetString(CarritoSessionKey, carritoJson);
        }

        public static void AgregarProducto(ISession session, Producto producto, int cantidad = 1)
        {
            var carrito = ObtenerCarrito(session);
            var itemExistente = carrito.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    ImagenUrl = producto.ImagenUrl,
                    Precio = producto.Precio,
                    Cantidad = cantidad,
                    StockDisponible = producto.Stock
                });
            }

            GuardarCarrito(session, carrito);
        }

        public static void ActualizarCantidad(ISession session, int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito(session);
            var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);

            if (item != null)
            {
                item.Cantidad = cantidad;
                GuardarCarrito(session, carrito);
            }
        }

        public static void EliminarProducto(ISession session, int productoId)
        {
            var carrito = ObtenerCarrito(session);
            carrito.RemoveAll(x => x.ProductoId == productoId);
            GuardarCarrito(session, carrito);
        }

        public static void LimpiarCarrito(ISession session)
        {
            session.Remove(CarritoSessionKey);
        }

        public static int ObtenerCantidadTotal(ISession session)
        {
            var carrito = ObtenerCarrito(session);
            return carrito.Sum(x => x.Cantidad);
        }

        public static decimal CalcularSubtotal(ISession session)
        {
            var carrito = ObtenerCarrito(session);
            return carrito.Sum(x => x.Subtotal);
        }

        public static decimal CalcularImpuesto(ISession session, decimal porcentajeImpuesto = 18m)
        {
            var subtotal = CalcularSubtotal(session);
            return subtotal * (porcentajeImpuesto / 100m);
        }

        public static decimal CalcularTotal(ISession session, decimal porcentajeImpuesto = 18m)
        {
            var subtotal = CalcularSubtotal(session);
            var impuesto = CalcularImpuesto(session, porcentajeImpuesto);
            return subtotal + impuesto;
        }
    }
}

