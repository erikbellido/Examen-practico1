using AutoMapper;
using BoticaOnlineAPI.DTOs;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.Repositories;

namespace BoticaOnlineAPI.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repo;
        private readonly IMapper _mapper;

        public PedidoService(IPedidoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PedidoDTO>> ObtenerPedidosAsync()
        {
            var pedidos = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<PedidoDTO>>(pedidos);
        }

        public async Task<PedidoDTO?> ObtenerPedidoPorIdAsync(int id)
        {
            var pedido = await _repo.GetByIdAsync(id);
            return pedido == null ? null : _mapper.Map<PedidoDTO>(pedido);
        }

        public async Task CrearPedidoAsync(PedidoDTO dto)
        {
            var pedido = _mapper.Map<Pedido>(dto);
            await _repo.AddAsync(pedido);
        }

        public async Task ActualizarEstadoAsync(int id, string nuevoEstado)
        {
            var pedido = await _repo.GetByIdAsync(id);
            if (pedido == null || pedido.Estado == "Entregado") return;

            pedido.Estado = nuevoEstado;
            await _repo.UpdateAsync(pedido);
        }

        public async Task CancelarPedidoAsync(int id)
        {
            await _repo.CancelAsync(id);
        }
    }
}