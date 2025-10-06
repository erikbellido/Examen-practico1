// FileName: /Examen1/BoticaOnlineAPI/Helpers/AutoMapperProfile.cs
using AutoMapper;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.DTOs;

namespace BoticaOnlineAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeo de Entidad a DTO de lectura
            CreateMap<Producto, ProductoDTO>();
            // Mapeo de DTO de creación/actualización a Entidad
            CreateMap<ProductoCrearDTO, Producto>(); 

            CreateMap<Cliente, ClienteDTO>();
            CreateMap<ClienteCrearDTO, Cliente>();
            
            CreateMap<PedidoCrearDTO, Pedido>();
            CreateMap<Pedido, PedidoDTO>();
            
            CreateMap<PrescripcionCrearDTO, Prescripcion>();
            CreateMap<Prescripcion, PrescripcionDTO>();
        }
    }
}