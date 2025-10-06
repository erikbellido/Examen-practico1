namespace BoticaOnlineAPI.DTOs
{
    public class PedidoCrearDTO
    {
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string DireccionEnvio { get; set; } = string.Empty;
        public int ClienteId { get; set; }
    }
}