using BoticaOnlineAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;
using BoticaOnlineAPI.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions; // Necesario para ProjectTo

namespace BoticaOnlineAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly BoticaDbContext _context;
        private readonly IMapper _mapper; // Inyectar IMapper

        public ProductosController(BoticaDbContext context, IMapper mapper) // Añadir IMapper al constructor
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Lista de productos activos con paginación y filtros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos( // Devolvemos una lista de DTOs
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? nombre = null,
            [FromQuery] string? categoria = null,
            [FromQuery] bool? activo = true)
        {
            try
            {
                IQueryable<Producto> query = _context.Productos;

                // Filter by active status
                if (activo.HasValue)
                {
                    query = query.Where(p => p.Activo == activo.Value);
                }

                // Filter by name
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }

                // Filter by category
                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    query = query.Where(p => p.Categoria.Contains(categoria));
                }

                // Apply pagination and project to DTOs
                var totalItems = await query.CountAsync();
                var productosDto = await PaginacionHelper.Paginar(query, page, pageSize)
                                                         .ProjectTo<ProductoDTO>(_mapper.ConfigurationProvider) // Proyección eficiente a DTOs
                                                         .ToListAsync();

                // Corrección: Usar indexador en lugar de Add para evitar warnings ASP0019
                Response.Headers["X-Total-Count"] = totalItems.ToString();
                Response.Headers["X-Page-Number"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(productosDto); // Devolvemos los DTOs
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al cargar productos", error = ex.Message });
            }
        }

        // GET: Producto por ID (solo si activo)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id) // Devolvemos un DTO
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null || !producto.Activo)
                    return NotFound(new { message = "Producto no encontrado o inactivo" });

                // Mapeamos la entidad a su DTO antes de devolverla
                var productoDto = _mapper.Map<ProductoDTO>(producto);
                return Ok(productoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar producto", error = ex.Message });
            }
        }

        // GET: Buscar productos por nombre/categoría (esta funcionalidad ya está cubierta por GetProductos)
        // Este método es redundante si GetProductos ya maneja los parámetros de búsqueda.
        // Lo he mantenido y adaptado para devolver DTOs, pero considera si realmente lo necesitas como un endpoint separado.
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> SearchProductos( // Devolvemos una lista de DTOs
            [FromQuery] string? query = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                IQueryable<Producto> productsQuery = _context.Productos.Where(p => p.Activo);

                if (!string.IsNullOrWhiteSpace(query))
                {
                    productsQuery = productsQuery.Where(p =>
                        p.Nombre.Contains(query) || p.Categoria.Contains(query) || p.Descripcion.Contains(query));
                }

                var totalItems = await productsQuery.CountAsync();
                var productsDto = await PaginacionHelper.Paginar(productsQuery, page, pageSize)
                                                        .ProjectTo<ProductoDTO>(_mapper.ConfigurationProvider) // Proyección eficiente a DTOs
                                                        .ToListAsync();

                // Corrección: Usar indexador en lugar de Add para evitar warnings ASP0019
                Response.Headers["X-Total-Count"] = totalItems.ToString();
                Response.Headers["X-Page-Number"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(productsDto); // Devolvemos los DTOs
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar productos", error = ex.Message });
            }
        }

        // POST: Crear nuevo producto (solo administradores)
        [HttpPost]
        // [Authorize(Roles = "Admin")] // Example of authorization, requires implementation of authentication/authorization
        public async Task<ActionResult<ProductoDTO>> CrearProducto([FromBody] ProductoCrearDTO productoDto) // Recibimos un DTO para crear
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Datos inválidos", errors = ModelState });
                }

                // Mapeamos el DTO a la entidad Producto
                var producto = _mapper.Map<Producto>(productoDto);

                if (string.IsNullOrWhiteSpace(producto.Nombre) || string.IsNullOrWhiteSpace(producto.Categoria))
                {
                    return BadRequest(new { message = "Nombre y Categoría son requeridos" });
                }
                if (producto.Precio <= 0)
                {
                    return BadRequest(new { message = "Precio debe ser mayor a 0" });
                }
                if (producto.Stock < 0)
                {
                    return BadRequest(new { message = "Stock debe ser 0 o mayor" });
                }

                producto.Activo = true; // Aseguramos que el producto se crea como activo
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                // Mapeamos la entidad creada de vuelta a un DTO para la respuesta
                var productoRespuestaDto = _mapper.Map<ProductoDTO>(producto);
                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, productoRespuestaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear producto", error = ex.Message });
            }
        }

        // PUT: Actualizar producto existente
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoCrearDTO productoDto) // Recibimos un DTO para actualizar
        {
            try
            {
                var productoExistente = await _context.Productos.FindAsync(id);
                if (productoExistente == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Datos inválidos", errors = ModelState });
                }

                // Mapeamos el DTO a la entidad existente
                _mapper.Map(productoDto, productoExistente);

                if (string.IsNullOrWhiteSpace(productoExistente.Nombre) || string.IsNullOrWhiteSpace(productoExistente.Categoria))
                {
                    return BadRequest(new { message = "Nombre y Categoría son requeridos" });
                }
                if (productoExistente.Precio <= 0)
                {
                    return BadRequest(new { message = "Precio debe ser mayor a 0" });
                }
                if (productoExistente.Stock < 0)
                {
                    return BadRequest(new { message = "Stock debe ser 0 o mayor" });
                }

                _context.Entry(productoExistente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar producto", error = ex.Message });
            }
        }

        // DELETE: Soft delete (marcar inactivo)
       [HttpDelete("{id}")]
public async Task<IActionResult> EliminarProducto(int id)
{
    try
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound(new { message = "Producto no encontrado" });
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error al eliminar producto", error = ex.Message });
    }
}

        // GET: Productos con stock bajo
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetLowStockProducts( // Devolvemos una lista de DTOs
            [FromQuery] int threshold = 5, // Default threshold for low stock
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                IQueryable<Producto> query = _context.Productos
                    .Where(p => p.Activo && p.Stock <= threshold);

                var totalItems = await query.CountAsync();
                var productsDto = await PaginacionHelper.Paginar(query, page, pageSize)
                                                     .ProjectTo<ProductoDTO>(_mapper.ConfigurationProvider) // Proyección eficiente a DTOs
                                                     .ToListAsync();

                // Corrección: Usar indexador en lugar de Add para evitar warnings ASP0019
                Response.Headers["X-Total-Count"] = totalItems.ToString();
                Response.Headers["X-Page-Number"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(productsDto); // Devolvemos los DTOs
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos con stock bajo", error = ex.Message });
            }
        }
    }
}