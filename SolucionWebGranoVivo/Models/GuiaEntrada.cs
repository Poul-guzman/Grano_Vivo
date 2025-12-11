using SolucionWebGranoVivo.Models;
using System.ComponentModel.DataAnnotations;

public class GuiaEntrada
{
    [Key]
    public string IdGuiaEntrada { get; set; } = string.Empty;

    [Display(Name = "Proveedor")]
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    public string Responsable { get; set; } = string.Empty;

    [Required]
    public string Estado { get; set; } = "Pendiente";

    public string Observaciones { get; set; } = string.Empty;

    public List<DetalleGuiaEntrada> Detalles { get; set; } = new();
}
