// FileName: /Examen1/BoticaOnlineAPI/DTOs/ProductoCrearDTO.cs
namespace BoticaOnlineAPI.DTOs
{
    public class ProductoCrearDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        // Activo podría ser opcional o tener un valor predeterminado en el controlador
        public bool Activo { get; set; } = true; 
    }
}