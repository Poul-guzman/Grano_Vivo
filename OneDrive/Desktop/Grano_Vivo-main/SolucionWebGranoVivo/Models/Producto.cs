using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore; 

namespace SolucionWebGranoVivo.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor que 0.")]
        [Precision(10, 2)] 
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El peso es obligatorio.")]
        [Precision(10, 2)] 
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "El tipo de tueste es obligatorio.")]
        [StringLength(50)]
        public string TipoTueste { get; set; }

        [Required(ErrorMessage = "El origen es obligatorio.")]
        [StringLength(100)]
        public string Origen { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Debe incluir una imagen del producto.")]
        [StringLength(255)]
        [Display(Name = "Imagen URL")]
        public string ImagenUrl { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(20)]
        public string Estado { get; set; }
    }
}
