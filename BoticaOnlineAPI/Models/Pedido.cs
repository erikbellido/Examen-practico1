namespace BoticaOnlineAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string DireccionEnvio { get; set; } = string.Empty;

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public ICollection<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
    }
}