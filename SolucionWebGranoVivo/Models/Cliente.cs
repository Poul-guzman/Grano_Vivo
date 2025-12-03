using System.ComponentModel.DataAnnotations;

namespace SolucionWebGranoVivo.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100)]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El RUC es obligatorio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener 11 dígitos.")]
        public string RUC { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }
}
