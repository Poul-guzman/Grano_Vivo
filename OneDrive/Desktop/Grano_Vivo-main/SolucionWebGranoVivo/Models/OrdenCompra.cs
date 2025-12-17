using System.ComponentModel.DataAnnotations;

namespace SolucionWebGranoVivo.Models
{
    public class OrdenCompra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty; 

        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public string Estado { get; set; } = "Pendiente";

        public decimal Total { get; set; }

        public List<DetalleOrdenCompra> Detalles { get; set; } = new();
    }
}
