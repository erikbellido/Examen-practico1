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
    public class PrescripcionesController : ControllerBase
    {
        private readonly BoticaDbContext _context;
        private readonly IMapper _mapper;

        public PrescripcionesController(BoticaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<PrescripcionDTO>> SubirPrescripcion(PrescripcionCrearDTO dto)
        {
            var prescripcion = _mapper.Map<Prescripcion>(dto);
            _context.Prescripciones.Add(prescripcion);
            await _context.SaveChangesAsync();

            var respuesta = _mapper.Map<PrescripcionDTO>(prescripcion);
            return CreatedAtAction(nameof(ObtenerPrescripcion), new { id = prescripcion.Id }, respuesta);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescripcionDTO>> ObtenerPrescripcion(int id)
        {
            var prescripcion = await _context.Prescripciones.FindAsync(id);
            if (prescripcion == null)
                return NotFound();

            var dto = _mapper.Map<PrescripcionDTO>(prescripcion);
            return Ok(dto);
        }

        [HttpPatch("{id}/validar")]
        public async Task<IActionResult> ValidarPrescripcion(int id)
        {
            var prescripcion = await _context.Prescripciones.FindAsync(id);
            if (prescripcion == null)
                return NotFound();

            prescripcion.Validada = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}