using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SolucionWebGranoVivo.Models
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(200)]
        public string? Descripcion { get; set; }
    }
}
