using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;
using SolucionWebGranoVivo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolucionWebGranoVivo.Pages.Admin.GuiasSalida
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuiaSalida GuiaSalida { get; set; } = new();

        [BindProperty]
        public List<DetalleGuiaSalida> Detalles { get; set; } = new List<DetalleGuiaSalida>();

        public List<SelectListItem> ClientesLista { get; set; }
        public List<SelectListItem> ProductosLista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasAsync();

            GuiaSalida.IdGuiaSalida = await GenerarIdGuiaAsync();
            GuiaSalida.Fecha = System.DateTime.Now;
            GuiaSalida.Responsable = User?.Identity?.Name ?? "Sistema";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarListasAsync();

            if (GuiaSalida.ClienteId <= 0)
                ModelState.AddModelError("GuiaSalida.ClienteId", "Seleccione un cliente.");

            
            Detalles = Detalles?.Where(d => d.ProductoId > 0 && d.Cantidad > 0).ToList();

            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrWhiteSpace(GuiaSalida.IdGuiaSalida))
                GuiaSalida.IdGuiaSalida = await GenerarIdGuiaAsync();

            using var trx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.GuiaSalida.Add(GuiaSalida);
                await _context.SaveChangesAsync();

                foreach (var d in Detalles)
                {
                    d.IdGuiaSalida = GuiaSalida.IdGuiaSalida;
                    _context.DetalleGuiaSalida.Add(d);
                }

                if (Detalles.Any())
                    await _context.SaveChangesAsync();

                await trx.CommitAsync();
                return RedirectToPage("Index");
            }
            catch
            {
                await trx.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la guía.");
                return Page();
            }
        }

        private async Task CargarListasAsync()
        {
            ClientesLista = await _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombres + " " + c.Apellidos
                }).ToListAsync();

            ProductosLista = await _context.Productos
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToListAsync();
        }

        private async Task<string> GenerarIdGuiaAsync()
        {
            var ultimaGuia = await _context.GuiaSalida
                .OrderByDescending(g => g.IdGuiaSalida)
                .FirstOrDefaultAsync();

            int numero = 1;

            if (ultimaGuia != null && !string.IsNullOrEmpty(ultimaGuia.IdGuiaSalida))
            {
                string suf = ultimaGuia.IdGuiaSalida.Substring(3);
                if (int.TryParse(suf, out var n))
                    numero = n + 1;
            }

            return $"GUI{numero:D3}";
        }
    }
}
