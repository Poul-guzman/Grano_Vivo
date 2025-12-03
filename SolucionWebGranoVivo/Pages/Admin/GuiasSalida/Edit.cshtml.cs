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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuiaSalida GuiaSalida { get; set; } = new();

        [BindProperty]
        public List<DetalleGuiaSalida> Detalles { get; set; } = new();

        public List<SelectListItem> ClientesLista { get; set; } = new();
        public List<SelectListItem> ProductosLista { get; set; } = new();

        public async Task<IActionResult> OnGetAsync([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            GuiaSalida = await _context.GuiaSalida
                .Include(g => g.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(g => g.IdGuiaSalida == id);

            if (GuiaSalida == null)
                return NotFound();

            Detalles = GuiaSalida.Detalles.ToList();
            await LoadLists();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLists();
                return Page();
            }

            var guiaExistente = await _context.GuiaSalida
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.IdGuiaSalida == GuiaSalida.IdGuiaSalida);

            if (guiaExistente == null)
                return NotFound();

            
            guiaExistente.ClienteId = GuiaSalida.ClienteId;
            guiaExistente.Fecha = GuiaSalida.Fecha;
            guiaExistente.Responsable = GuiaSalida.Responsable;
            guiaExistente.Observaciones = GuiaSalida.Observaciones;

            
            var detallesExistentes = guiaExistente.Detalles.ToList();

           
            foreach (var det in detallesExistentes)
            {
                if (!Detalles.Any(d => d.Id == det.Id))
                {
                    _context.DetalleGuiaSalida.Remove(det);
                }
            }

            
            foreach (var det in Detalles)
            {
                if (det.Id > 0)
                {
                    
                    var detalleDB = detallesExistentes.FirstOrDefault(d => d.Id == det.Id);
                    if (detalleDB != null)
                    {
                        detalleDB.ProductoId = det.ProductoId;
                        detalleDB.Cantidad = det.Cantidad;
                        
                    }
                }
                else
                {
                    
                    det.IdGuiaSalida = guiaExistente.IdGuiaSalida;
                    _context.DetalleGuiaSalida.Add(det);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        private async Task LoadLists()
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
    }
}
