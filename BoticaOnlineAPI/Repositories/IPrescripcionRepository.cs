using BoticaOnlineAPI.Models;

namespace BoticaOnlineAPI.Repositories
{
    public interface IPrescripcionRepository
    {
        Task<Prescripcion?> GetByIdAsync(int id);
        Task AddAsync(Prescripcion prescripcion);
        Task ValidarAsync(int id);
    }
}
