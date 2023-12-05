using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Catalogos;
using ProyectoApiContable.Entities;
using ProyectoApiContable.Services.Autentication;
using ProyectoApiContable.Services;
using Microsoft.Data.SqlClient;

namespace ProyectoApiContable.Controllers;

[Route("api/Cuentas")]
[ApiController]
[Authorize]
public class CuentasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRedisServices _redisServices;
    private readonly IUserContextService _userContextService;

    public CuentasController(ApplicationDbContext context, 
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
    public async Task<ActionResult<ResponseDto<IReadOnlyList<CuentaDto>>>> ObtenerCuentas()
    {
        var catalogosDb = await _context.Cuentas.ToListAsync();
        var catalogosDto = _mapper.Map<List<CuentaDto>>(catalogosDb);
        return Ok(new ResponseDto<List<CuentaDto>>
        {
            Status = true,
            Data = catalogosDto
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> MostrarCuenta(Guid id)
    {
        var cuenta = await _context.Cuentas.FindAsync(id);

        if (cuenta == null)
        {
            var notFoundResponse = new ResponseDto<CuentaDto>
            {
                Status = false,
                Message = "Cuenta no encontrada.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        var cuentaDto = _mapper.Map<CuentaDto>(cuenta);

        var successResponse = new ResponseDto<CuentaDto>
        {
            Status = true,
            Message = "Cuenta recuperada correctamente.",
            Data = cuentaDto
        };

        return Ok(successResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CrearCuenta([FromBody] CreateCuentaDto createCuentaDto)
    {
        if (createCuentaDto == null)
        {
            var badRequestResponse = new ResponseDto<CuentaDto>
            {
                Status = false,
                Message = "Datos de la cuenta no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        try
        {
            // Mapear el CreateCuentaDto a una entidad Cuenta
            var cuenta = _mapper.Map<Cuenta>(createCuentaDto);
            cuenta.FechaCreacion = DateTime.Now;

            // Guardar la cuenta en la base de datos
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            // Retornar una respuesta con la cuenta creada
            var cuentaCreadaDto = _mapper.Map<CuentaDto>(cuenta);

            // Obtener usuario 
            var usuarioActual = await _userContextService.GetUserAsync();

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            // Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} Creo una nueva cuenta: " +
                $"Nombre: {cuenta.Nombre} " +
                $"Id: {cuenta.Id} " +
                $"Saldo: {cuenta.Saldo} " +
                $"- [{fechaActual}]");

            var successResponse = new ResponseDto<CuentaDto>
            {
                Status = true,
                Message = "Cuenta creada correctamente.",
                Data = cuentaCreadaDto
            };

            return CreatedAtAction(nameof(MostrarCuenta), new { id = cuenta.Id }, successResponse);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlException && sqlException.Number == 2601)
            {
                // manejar error de codigo no unico
                var errorResponse = new ResponseDto<CuentaDto>
                {
                    Status = false,
                    Message = "¡El código no es único! Ya existe en la base de datos.",
                    Data = null
                };

                return Conflict(errorResponse);
            }
            else
            {
                return StatusCode(500, "Error al procesar la solicitud");
            }
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarCuenta(Guid id, [FromBody] UpdateCuentaDto updateCuentaDto)
    {
        if (updateCuentaDto == null)
        {
            var badRequestResponse = new ResponseDto<CuentaDto>
            {
                Status = false,
                Message = "Datos de la cuenta no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        var cuenta = await _context.Cuentas.FindAsync(id);

        if (cuenta == null)
        {
            var notFoundResponse = new ResponseDto<CuentaDto>
            {
                Status = false,
                Message = "Cuenta no encontrada.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        // Mapear el UpdateCuentaDto a la entidad Cuenta
        _mapper.Map(updateCuentaDto, cuenta);

        // Actualizar la cuenta en la base de datos
        _context.Cuentas.Update(cuenta);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con la cuenta actualizada
        var cuentaActualizadaDto = _mapper.Map<CuentaDto>(cuenta);

        //Obtener usuario 
        var usuarioActual = await _userContextService.GetUserAsync();

        // Obtener la fecha y hora actual
        var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

        //Agregar el log en redis
        await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} actualizo una cuenta: " +
            $"Nombre: {cuenta.Nombre} " +
            $"Id: {cuenta.Id} " +
            $"Saldo: {cuenta.Saldo} " +
            $"- [{fechaActual}]");

        var successResponse = new ResponseDto<CuentaDto>
        {
            Status = true,
            Message = "Cuenta actualizada correctamente.",
            Data = cuentaActualizadaDto
        };

        return Ok(successResponse);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCuenta(Guid id)
    {
        var cuenta = await _context.Cuentas.FindAsync(id);

        if (cuenta == null)
        {
            var notFoundResponse = new ResponseDto<CuentaDto>
            {
                Status = false,
                Message = "Cuenta no encontrada.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        // Eliminar la cuenta de la base de datos
        _context.Cuentas.Remove(cuenta);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con la cuenta eliminada
        var cuentaEliminadaDto = _mapper.Map<CuentaDto>(cuenta);

        //Obtener usuario 
        var usuarioActual = await _userContextService.GetUserAsync();

        // Obtener la fecha y hora actual
        var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

        //Agregar el log en redis
        await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} borro una cuenta: " +
            $"Nombre: {cuenta.Nombre} " +
            $"Id: {cuenta.Id} " +
            $"Saldo: {cuenta.Saldo} " +
            $"- [{fechaActual}]");

        var successResponse = new ResponseDto<CuentaDto>
        {
            Status = true,
            Message = "Cuenta eliminada correctamente.",
            Data = cuentaEliminadaDto
        };

        return Ok(successResponse);
    }
}