using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaOnlineAPI.Repositories
{
    public class PrescripcionRepository : IPrescripcionRepository
    {
        private readonly BoticaDbContext _context;

        public PrescripcionRepository(BoticaDbContext context)
        {
            _context = context;
        }

        public async Task<Prescripcion?> GetByIdAsync(int id)
        {
            return await _context.Prescripciones.FindAsync(id);
        }

        public async Task AddAsync(Prescripcion prescripcion)
        {
            _context.Prescripciones.Add(prescripcion);
            await _context.SaveChangesAsync();
        }

        public async Task ValidarAsync(int id)
        {
            var prescripcion = await _context.Prescripciones.FindAsync(id);
            if (prescripcion != null)
            {
                prescripcion.Validada = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}