using AutoMapper;
using BoticaOnlineAPI.DTOs;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.Repositories;

namespace BoticaOnlineAPI.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClienteDTO>> ObtenerClientesAsync()
        {
            var clientes = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteDTO>>(clientes);
        }

        public async Task<ClienteDTO?> ObtenerClientePorIdAsync(int id)
        {
            var cliente = await _repo.GetByIdAsync(id);
            return cliente == null ? null : _mapper.Map<ClienteDTO>(cliente);
        }

        public async Task CrearClienteAsync(ClienteDTO dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);
            await _repo.AddAsync(cliente);
        }

        public async Task ActualizarClienteAsync(int id, ClienteDTO dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);
            cliente.Id = id;
            await _repo.UpdateAsync(cliente);
        }

        public async Task EliminarClienteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}