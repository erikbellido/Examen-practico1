using System.ComponentModel.DataAnnotations.Schema;
namespace BoticaOnlineAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
         [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; } = true;
    }
}