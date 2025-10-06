namespace BoticaOnlineAPI.Models
{
    public class Prescripcion
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Validada { get; set; } = false;
       

        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

    }
}


