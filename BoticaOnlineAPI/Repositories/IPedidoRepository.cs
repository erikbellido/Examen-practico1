using BoticaOnlineAPI.Models;

namespace BoticaOnlineAPI.Repositories
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(int id);
        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task CancelAsync(int id);
    }
}