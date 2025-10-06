using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaOnlineAPI.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly BoticaDbContext _context;

        public PedidoRepository(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _context.Pedidos.ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _context.Pedidos.FindAsync(id);
        }

        public async Task AddAsync(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                pedido.Estado = "Cancelado";
                await _context.SaveChangesAsync();
            }
        }
    }
}