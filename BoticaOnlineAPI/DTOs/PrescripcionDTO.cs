namespace BoticaOnlineAPI.DTOs
{
    public class PrescripcionDTO
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Validada { get; set; }
        
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
    }
}