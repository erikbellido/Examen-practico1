using BoticaOnlineAPI.DTOs;

namespace BoticaOnlineAPI.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDTO>> ObtenerPedidosAsync();
        Task<PedidoDTO?> ObtenerPedidoPorIdAsync(int id);
        Task CrearPedidoAsync(PedidoDTO dto);
        Task ActualizarEstadoAsync(int id, string nuevoEstado);
        Task CancelarPedidoAsync(int id);
    }
}