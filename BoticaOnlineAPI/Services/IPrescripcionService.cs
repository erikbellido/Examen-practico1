using BoticaOnlineAPI.DTOs;

namespace BoticaOnlineAPI.Services
{
    public interface IPrescripcionService
    {
        Task<PrescripcionDTO?> ObtenerPrescripcionPorIdAsync(int id);
        Task SubirPrescripcionAsync(PrescripcionDTO dto);
        Task ValidarPrescripcionAsync(int id);
    }
}