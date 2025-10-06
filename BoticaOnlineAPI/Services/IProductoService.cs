using BoticaOnlineAPI.DTOs;

namespace BoticaOnlineAPI.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> ObtenerProductosAsync();
        Task<ProductoDTO?> ObtenerProductoPorIdAsync(int id);
        Task CrearProductoAsync(ProductoDTO dto);
        Task ActualizarProductoAsync(int id, ProductoDTO dto);
        Task EliminarProductoAsync(int id);
    }
}