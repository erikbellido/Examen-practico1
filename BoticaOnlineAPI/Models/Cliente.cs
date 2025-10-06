using BoticaOnlineAPI.Models;
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    // Propiedades de navegación internas
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
}
