using System.ComponentModel.DataAnnotations;

namespace SolucionWebGranoVivo.Models
{
    public class ReporteCompraGenerado
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(450)]
        public string UsuarioId { get; set; } = string.Empty;

        public int? ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [StringLength(500)]
        public string ParametrosUsados { get; set; } = string.Empty;
    }
}

