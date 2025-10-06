using BoticaOnlineAPI.DTOs;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.Repositories;

namespace BoticaOnlineAPI.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repo;

        public ProductoService(IProductoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerProductosAsync()
        {
            var productos = await _repo.GetAllAsync();
            return productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Categoria = p.Categoria,
                Precio = p.Precio,
                Stock = p.Stock
            });
        }

        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Categoria = p.Categoria,
                Precio = p.Precio,
                Stock = p.Stock
            };
        }

        public async Task CrearProductoAsync(ProductoDTO dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Categoria = dto.Categoria,
                Precio = dto.Precio,
                Stock = dto.Stock
            };
            await _repo.AddAsync(producto);
        }

        public async Task ActualizarProductoAsync(int id, ProductoDTO dto)
        {
            var producto = new Producto
            {
                Id = id,
                Nombre = dto.Nombre,
                Categoria = dto.Categoria,
                Precio = dto.Precio,
                Stock = dto.Stock
            };
            await _repo.UpdateAsync(producto);
        }

        public async Task EliminarProductoAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}