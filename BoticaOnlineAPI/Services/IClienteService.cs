using BoticaOnlineAPI.DTOs;

namespace BoticaOnlineAPI.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> ObtenerClientesAsync();
        Task<ClienteDTO?> ObtenerClientePorIdAsync(int id);
        Task CrearClienteAsync(ClienteDTO dto);
        Task ActualizarClienteAsync(int id, ClienteDTO dto);
        Task EliminarClienteAsync(int id);
    }
 }