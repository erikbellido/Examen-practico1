using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaOnlineAPI.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly BoticaDbContext _context;

        public ClienteRepository(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.Where(c => c.Activo).ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}