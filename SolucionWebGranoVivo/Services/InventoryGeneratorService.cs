// Services/InventoryGeneratorService.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SolucionWebGranoVivo.Data;

namespace SolucionWebGranoVivo.Services
{
    public class InventoryGeneratorService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _exportPath = Path.Combine("Exports", "Inventario");

        public InventoryGeneratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateAsync()
        {
            Directory.CreateDirectory(_exportPath);
            var file = Path.Combine(_exportPath, $"inventario_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            var productos = await _context.Productos
                .Select(p => $"{p.Nombre},{p.Codigo},{p.Stock},{p.Precio}")
                .ToListAsync();

            var lines = new[] { "Producto,Codigo,Stock,Precio" }.Concat(productos);
            await File.WriteAllLinesAsync(file, lines);
            return file;
        }
    }
}
