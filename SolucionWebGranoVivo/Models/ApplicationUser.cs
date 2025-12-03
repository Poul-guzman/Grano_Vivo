using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SolucionWebGranoVivo.Models
{

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; } = string.Empty;


        public string? Rol { get; set; }
    }
}
