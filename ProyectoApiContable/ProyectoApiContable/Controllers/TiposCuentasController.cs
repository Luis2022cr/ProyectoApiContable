using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.TiposCuentas;
using ProyectoApiContable.Entities;
using ProyectoApiContable.Services;
using ProyectoApiContable.Services.Autentication;

namespace ProyectoApiContable.Controllers;

    [Route("api/TiposCuentas")]
    [ApiController]
    [Authorize]
    public class TiposCuentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRedisServices _redisServices;
        private readonly IUserContextService _userContextService;

    public TiposCuentasController(ApplicationDbContext context, 
            IMapper mapper,
            IRedisServices redisServices,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _redisServices = redisServices;
            _userContextService = userContextService;
    }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<TiposCuentaDto>>>> MostrarTiposCuentas()
        {
            var tiposCuentasDb = await _context.TiposCuentas.ToListAsync();
            var tiposCuentasDto = _mapper.Map<List<TiposCuentaDto>>(tiposCuentasDb);

            return Ok(new ResponseDto<List<TiposCuentaDto>>
            {
                Status = true,
                Data = tiposCuentasDto
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<TiposCuentaDto>>> MostrarTipoCuenta(int id)
        {
            var tipoCuentaDb = await _context.TiposCuentas.FindAsync(id);

            if (tipoCuentaDb == null)
            {
                var notFoundResponse = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "Tipo de cuenta no encontrada.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            var tipoCuentaDto = _mapper.Map<TiposCuentaDto>(tipoCuentaDb);

            return Ok(new ResponseDto<TiposCuentaDto>
            {
                Status = true,
                Data = tipoCuentaDto
            });
        }

        
        [HttpPost]
        public async Task<IActionResult> CrearTipoCuenta([FromBody] CreateTiposCuentaDto createTipoCuentaDto)
        {
            if (createTipoCuentaDto == null)
            {
                var badRequestResponse = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "Datos del tipo de cuenta no válidos.",
                    Data = null
                };

                return BadRequest(badRequestResponse);
            }

            // Mapear el CreateTipoCuentaDto a una entidad TipoCuenta
            var tipoCuenta = _mapper.Map<TipoCuenta>(createTipoCuentaDto);

            // Guardar el tipo de cuenta en la base de datos
            _context.TiposCuentas.Add(tipoCuenta);
            await _context.SaveChangesAsync();

            // Retornar una respuesta con el tipo de cuenta creado
            var tipoCuentaCreadoDto = _mapper.Map<TiposCuentaDto>(tipoCuenta);
            
            //Obtener usuario 
            var usuarioActual = await _userContextService.GetUserAsync();

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} Creo un tipo de cuenta: " +
                $"Nombre: {tipoCuentaCreadoDto.Nombre} " +
                $"Id: {tipoCuentaCreadoDto.Id} " +
                $"- [{fechaActual}]");

            var successResponse = new ResponseDto<TiposCuentaDto>
            {
                Status = true,
                Message = "Tipo de cuenta creada correctamente.",
                Data = tipoCuentaCreadoDto
            };

            return CreatedAtAction(nameof(MostrarTipoCuenta), new { id = tipoCuenta.Id }, successResponse);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTipoCuenta(int id, [FromBody] UpdateTiposCuentaDto updateTipoCuentaDto)
        {
            if (updateTipoCuentaDto == null)
            {
                var badRequestResponse = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "Datos del tipo de cuenta no válidos.",
                    Data = null
                };

                return BadRequest(badRequestResponse);
            }

            var tipoCuenta = await _context.TiposCuentas.FindAsync(id);

            if (tipoCuenta == null)
            {
                var notFoundResponse = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "Tipo de cuenta no encontrada.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            // Mapear el UpdateTipoCuentaDto a la entidad TipoCuenta
            _mapper.Map(updateTipoCuentaDto, tipoCuenta);

            // Actualizar el tipo de cuenta en la base de datos
            _context.TiposCuentas.Update(tipoCuenta);
            await _context.SaveChangesAsync();

            // Retornar una respuesta con el tipo de cuenta actualizado
            var tipoCuentaActualizadoDto = _mapper.Map<TiposCuentaDto>(tipoCuenta);
            
            //Obtener usuario 
            var usuarioActual = await _userContextService.GetUserAsync();

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} Actualizo un tipo de cuenta: " +
                $"Nombre: {tipoCuentaActualizadoDto.Nombre} " +
                $"Id: {tipoCuentaActualizadoDto.Id} " +
                $"- [{fechaActual}]");

            var successResponse = new ResponseDto<TiposCuentaDto>
            {
                Status = true,
                Message = "Tipo de cuenta actualizada correctamente.",
                Data = tipoCuentaActualizadoDto
            };

            return Ok(successResponse);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTipoCuenta(int id)
        {
            var tipoCuenta = await _context.TiposCuentas.FindAsync(id);

            if (tipoCuenta == null)
            {
                var notFoundResponse = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "Tipo de cuenta no encontrada.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }
            
            var estaAsociadaACuentass = _context.Cuentas
            .Any(p => p.TipoCuentaId == id);

            if (estaAsociadaACuentass)
            {
                var response = new ResponseDto<TiposCuentaDto>
                {
                    Status = false,
                    Message = "No se puede eliminar el tipo de cuenta porque está asociada a una o más cuentas.",
                    Data = null
                };

                return BadRequest(response);
            }

            // Eliminar el tipo de cuenta de la base de datos
            _context.TiposCuentas.Remove(tipoCuenta);
            await _context.SaveChangesAsync();

            // Retornar una respuesta con el tipo de cuenta eliminado
            var tipoCuentaEliminadoDto = _mapper.Map<TiposCuentaDto>(tipoCuenta);

            //Obtener usuario 
            var usuarioActual = await _userContextService.GetUserAsync();

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} Borro un tipo de cuenta: " +
                $"Nombre: {tipoCuentaEliminadoDto.Nombre} " +
                $"Id: {tipoCuentaEliminadoDto.Id} " +
                $"- [{fechaActual}]");  

            var successResponse = new ResponseDto<TiposCuentaDto>
            {
                Status = true,
                Message = "Tipo de cuenta eliminada correctamente.",
                Data = tipoCuentaEliminadoDto
            };

            return Ok(successResponse);
        }
    }

