using AutoMapper;
using BoticaOnlineAPI.DTOs;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.Repositories;

namespace BoticaOnlineAPI.Services
{
    public class PrescripcionService : IPrescripcionService
    {
        private readonly IPrescripcionRepository _repo;
        private readonly IMapper _mapper;

        public PrescripcionService(IPrescripcionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PrescripcionDTO?> ObtenerPrescripcionPorIdAsync(int id)
        {
            var prescripcion = await _repo.GetByIdAsync(id);
            return prescripcion == null ? null : _mapper.Map<PrescripcionDTO>(prescripcion);
        }

        public async Task SubirPrescripcionAsync(PrescripcionDTO dto)
        {
            var prescripcion = _mapper.Map<Prescripcion>(dto);
            await _repo.AddAsync(prescripcion);
        }

        public async Task ValidarPrescripcionAsync(int id)
        {
            await _repo.ValidarAsync(id);
        }
    }
}