using Microsoft.AspNetCore.Mvc.ModelBinding;
using SolucionWebGranoVivo.Models;
using System.ComponentModel.DataAnnotations;

public class GuiaSalida
{
    [Key]
    public string IdGuiaSalida { get; set; } = string.Empty;

    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    public string Responsable { get; set; } = string.Empty;

    [Required]
    public string Estado { get; set; } = "Pendiente";

    public string Observaciones { get; set; } = string.Empty;

    public List<DetalleGuiaSalida> Detalles { get; set; } = new();
}
