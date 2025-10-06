using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaOnlineAPI.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly BoticaDbContext _context;

        public ProductoRepository(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos.Where(p => p.Activo).ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}