using SolucionWebGranoVivo.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DetalleGuiaEntrada
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string IdGuiaEntrada { get; set; } = string.Empty;

    [ForeignKey(nameof(IdGuiaEntrada))]
    public GuiaEntrada? GuiaEntrada { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }
}
