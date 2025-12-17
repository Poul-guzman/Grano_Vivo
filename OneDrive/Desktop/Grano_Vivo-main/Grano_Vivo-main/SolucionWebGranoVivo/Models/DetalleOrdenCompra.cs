using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolucionWebGranoVivo.Models
{
    public class DetalleOrdenCompra
    {
        [Key]
        public int Id { get; set; }

        public int OrdenCompraId { get; set; }
        [ForeignKey(nameof(OrdenCompraId))]
        public OrdenCompra? OrdenCompra { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }

        [Precision(10, 2)]
        public decimal PrecioUnitario { get; set; }

        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
    }
}
