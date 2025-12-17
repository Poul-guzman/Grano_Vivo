using SolucionWebGranoVivo.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DetalleGuiaSalida
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string IdGuiaSalida { get; set; } = string.Empty;

    [ForeignKey(nameof(IdGuiaSalida))]
    public GuiaSalida? GuiaSalida { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }
}
