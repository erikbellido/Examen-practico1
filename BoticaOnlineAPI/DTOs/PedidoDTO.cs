namespace BoticaOnlineAPI.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string DireccionEnvio { get; set; } = string.Empty;
        public int ClienteId { get; set; }
    }
}