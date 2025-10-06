namespace BoticaOnlineAPI.DTOs
{
    public class PrescripcionCrearDTO
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Validada { get; set; } = false;
        
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
    }
}