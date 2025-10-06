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
    public class ClientesController : ControllerBase
    {
        private readonly BoticaDbContext _context;
        private readonly IMapper _mapper;

        public ClientesController(BoticaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> RegistrarCliente(ClienteCrearDTO clienteDTO)
        {
            var cliente = _mapper.Map<Cliente>(clienteDTO);
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var clienteRespuesta = _mapper.Map<ClienteDTO>(cliente);
            return CreatedAtAction(nameof(ObtenerCliente), new { id = cliente.Id }, clienteRespuesta);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> ObtenerCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null || !cliente.Activo)
                return NotFound();

            var clienteDTO = _mapper.Map<ClienteDTO>(cliente);
            return Ok(clienteDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCliente(int id, ClienteCrearDTO clienteDTO)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente == null)
                return NotFound();

            _mapper.Map(clienteDTO, clienteExistente);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/pedidos")]
        public async Task<ActionResult<IEnumerable<Pedido>>> HistorialPedidos(int id)
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == id)
                .ToListAsync();

            return pedidos;
        }
    }
}