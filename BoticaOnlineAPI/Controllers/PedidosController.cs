using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using BoticaOnlineAPI.DTOs;
using AutoMapper;

namespace BoticaOnlineAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly BoticaDbContext _context;
        private readonly IMapper _mapper;

        public PedidosController(BoticaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<PedidoDTO>> CrearPedido(PedidoCrearDTO pedidoDTO)
        {
            if (string.IsNullOrEmpty(pedidoDTO.DireccionEnvio))
                return BadRequest(new { message = "La dirección de envío es obligatoria." });

            var pedido = _mapper.Map<Pedido>(pedidoDTO);
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var pedidoRespuesta = _mapper.Map<PedidoDTO>(pedido);
            return CreatedAtAction(nameof(ObtenerPedido), new { id = pedido.Id }, pedidoRespuesta);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDTO>> ObtenerPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            var pedidoDTO = _mapper.Map<PedidoDTO>(pedido);
            return Ok(pedidoDTO);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            if (pedido.Estado == "Entregado")
                return Conflict(new { message = "No se puede modificar un pedido ya entregado." });

            pedido.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            pedido.Estado = "Cancelado";
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}