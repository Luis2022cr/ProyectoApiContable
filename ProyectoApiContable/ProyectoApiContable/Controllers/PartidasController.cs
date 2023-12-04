using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.FilasPartidas;
using ProyectoApiContable.Dtos.Partidas;
using ProyectoApiContable.Entities;
using ProyectoApiContable.Services;
using ProyectoApiContable.Services.Autentication;

namespace ProyectoApiContable.Controllers;

[Route("api/partidas")]
[ApiController]
[Authorize]
public class PartidasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRedisServices _redisServices;
    private readonly IUserContextService _userContextService;

    public PartidasController(ApplicationDbContext context, 
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
    public async Task<IActionResult> MostrarTodasLasPartidas()
    {
        try
        {
            var partidas = await _context.Partidas
                .Include(p => p.FilasPartida)
                .Include(p => p.CreadoPor)
                .Include(p => p.RevisadoPor) 
                .ToListAsync();

            var partidasDto = _mapper.Map<List<PartidaDto>>(partidas);

            var response = new ResponseDto<List<PartidaDto>>
            {
                Status = true,
                Message = "Partidas recuperadas correctamente.",
                Data = partidasDto
            };

            return Ok(response);
        }
        catch (Exception)
        {
            // Log the exception
            return StatusCode(500, new ResponseDto<List<PartidaDto>>
            {
                Status = false,
                Message = "Se produjo un error al recuperar las partidas.",
                Data = null
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> MostrarPartida(Guid id)
    {
        try
        {
            var partida = await _context.Partidas
                .Include(p => p.FilasPartida)
                .Include(p => p.CreadoPor)
                .Include(p => p.RevisadoPor)
                .ToListAsync();

            if (partida == null)
            {
                var notFoundResponse = new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "Partida no encontrada.",
                    Data = null
                };

                return NotFound(notFoundResponse);
            }

            var partidaDto = _mapper.Map<PartidaDto>(partida);

            var successResponse = new ResponseDto<PartidaDto>
            {
                Status = true,
                Message = "Partida recuperada correctamente.",
                Data = partidaDto
            };

            return Ok(successResponse);
        }
        catch (Exception)
        {
            // Log the exception
            return StatusCode(500, new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "Se produjo un error al recuperar la partida.",
                Data = null
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Empleado")]
    public async Task<IActionResult> CrearPartida([FromBody] CreatePartidaDto createPartidaDto)
    {
        try
        {
            if (createPartidaDto == null || !ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "Datos de la partida no válidos.",
                    Data = null
                });
            }
            // Validar que el número de filas sea par
            if (createPartidaDto.FilasPartida.Count < 2)
            {
                return BadRequest(new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "El número de filas en la partida debe ser 2 o mas.",
                    Data = null
                });
            }

            var partida = _mapper.Map<Partida>(createPartidaDto);
            var partidaId = Guid.NewGuid();
            partida.Id = partidaId;

            var nuevasFilasPartida = new List<FilasPartida>();
            foreach (var filasDto in partida.FilasPartida)
            {
                // Asignar un nuevo GUID a cada fila de la partida
                filasDto.Id = Guid.NewGuid();
                filasDto.PartidaId = partidaId;

                var filaPartida = _mapper.Map<FilasPartida>(filasDto);
                _context.FilasPartidas.Add(filaPartida);

                // Asigna la fila de la partida a la colección de la partida
                nuevasFilasPartida.Add(filaPartida);
            }

            partida.FilasPartida = nuevasFilasPartida;

            partida.FechaCreacion = DateTime.Now;
            partida.EstadoPartidaId = 2;

            var usuarioActual = await _userContextService.GetUserAsync();

            if (usuarioActual != null)
            {
                partida.CreadoPorId = usuarioActual.Id;
            }
            else
            {
                return BadRequest(new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "No se pudo encontrar el usuario actual.",
                    Data = null
                });
            }

            _context.Partidas.Add(partida);

            await _context.SaveChangesAsync();

            var partidaCreadaDto = _mapper.Map<PartidaDto>(partida);

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            string logMessage = $"El usuario: {usuarioActual} Ingreso una partida: " +
                                $"Nombre Partida: {partidaCreadaDto.Nombre} " +
                                $"Id: {partidaCreadaDto.Id} ";

            foreach (var fila in partidaCreadaDto.FilasPartida)
            {
                logMessage +=
                    $"CuentaId: {fila.CuentaId} " +
                    $"Debito: {fila.Debito} " +
                    $"Credito: {fila.Credito} ";
            }

            logMessage += $"- [{fechaActual}]";
            await _redisServices.AgregarLogARedis(logMessage);

            return CreatedAtAction(nameof(MostrarPartida), new { id = partida.Id }, new ResponseDto<PartidaDto>
            {
                Status = true,
                Message = "Partida creada correctamente.",
                Data = partidaCreadaDto
            });
        }
        catch (Exception )
        {
            // Log the exception
            return StatusCode(500, new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "Se produjo un error al crear la partida.",
                Data = null
            });
        }
    }

    //endpoint para aprobar partidas
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/Aprobar")]
    public async Task<IActionResult> AprobarPartida(Guid id, [FromBody] EstadoPartidaDto estadoPartidaDto)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                if (estadoPartidaDto == null || !ModelState.IsValid)
                {
                    return BadRequest(new ResponseDto<PartidaDto>
                    {
                        Status = false,
                        Message = "Datos para aprobar la partida no válidos.",
                        Data = null
                    });
                }

                // Buscar la partida por su Id
                var partida = await _context.Partidas
                    .Include(p => p.FilasPartida)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (partida == null)
                {
                    return NotFound(new ResponseDto<PartidaDto>
                    {
                        Status = false,
                        Message = "Partida no encontrada.",
                        Data = null
                    });
                }

                var usuarioActual = await _userContextService.GetUserAsync();

                if (usuarioActual != null)
                {
                    partida.RevisadoPorId = usuarioActual.Id;
                }
                else
                {
                    return BadRequest(new ResponseDto<PartidaDto>
                    {
                        Status = false,
                        Message = "No se pudo encontrar el usuario actual.",
                        Data = null
                    });
                }

                // Validar que la suma de débitos y créditos en las filas de la partida sea igual
                if (!EsPartidaCuadrada(partida.FilasPartida))
                {
                    partida.EstadoPartidaId = 3;
                    partida.FechaRevision = DateTime.Now;

                    // Revertir la transacción en caso de una validación fallida
                    await transaction.RollbackAsync();

                    return BadRequest(new ResponseDto<PartidaDto>
                    {
                        Status = false,
                        Message = "La partida no está cuadrada. La suma de débitos y créditos no coincide. por lo que fue rechazada.",
                        Data = null
                    });
                }

                // Actualizar el estado de la partida utilizando el DTO
                partida.EstadoPartidaId = estadoPartidaDto.EstadoPartidaId;
                partida.FechaRevision = DateTime.Now;

                // Aplicar los cambios a las cuentas involucradas
                AplicarCambiosCuentas(partida.FilasPartida);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Confirmar la transacción si todas las operaciones fueron exitosas
                await transaction.CommitAsync();

                var partidaAprobadaDto = _mapper.Map<PartidaDto>(partida);

                // Obtener la fecha y hora actual
                var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

                //Agregar el log en redis
                await _redisServices.AgregarLogARedis($"El usuario: {usuarioActual} Aprobo la partida: " +
                    $"Nombre Partida: {partidaAprobadaDto.Nombre} " +
                    $"Id: {partidaAprobadaDto.Id} " +
                    $"- [{fechaActual}]");

                return Ok(new ResponseDto<PartidaDto>
                {
                    Status = true,
                    Message = "Partida aprobada correctamente.",
                    Data = partidaAprobadaDto
                });
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "Se produjo un error al aprobar la partida.",
                    Data = null
                });
            }
        }
    }

    private bool EsPartidaCuadrada(List<FilasPartida> filasPartida)
    {
        decimal sumaDebitos = filasPartida.Sum(f => f.Debito);
        decimal sumaCreditos = filasPartida.Sum(f => f.Credito);
        return sumaDebitos == sumaCreditos;
    }

    private void AplicarCambiosCuentas(List<FilasPartida> filasPartida)
    {
        foreach (var fila in filasPartida)
        {
            var cuenta = _context.Cuentas.Find(fila.CuentaId);

            if (cuenta != null)
            {
                // Actualizar saldo de la cuenta
                cuenta.Saldo += fila.Debito;
                cuenta.Saldo -= fila.Credito;


                // Guardar cambios en la cuenta
                _context.Entry(cuenta).State = EntityState.Modified;
            }
            else
            {
                
            }
        }

        // Guardar cambios en la base de datos
        _context.SaveChanges();
    }

    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPartida(Guid id)
    {
        try
        {
            // Buscar la partida por su Id
            var partida = await _context.Partidas.FindAsync(id);

            if (partida == null)
            {
                return NotFound(new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "Partida no encontrada.",
                    Data = null
                });
            }

            // Verificar si el estado de la partida es 3 (rechazado)
            if (partida.EstadoPartidaId != 3)
            {
                return BadRequest(new ResponseDto<PartidaDto>
                {
                    Status = false,
                    Message = "La partida no se puede eliminar porque no está en estado 'Rechazado'.",
                    Data = null
                });
            }

            // Eliminar la partida
            _context.Partidas.Remove(partida);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            //Obtener usuario 
            var usuarioActual = await _userContextService.GetUserAsync();

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            string logMessage = $"El usuario: {usuarioActual} Ingreso una partida: " +
                                $"Nombre Partida: {partida.Nombre} " +
                                $"Id: {partida.Id} ";

            foreach (var fila in partida.FilasPartida)
            {
                logMessage +=
                    $"CuentaId: {fila.CuentaId} " +
                    $"Debito: {fila.Debito} " +
                    $"Credito: {fila.Credito} ";
            }

            logMessage += $"- [{fechaActual}]";
            await _redisServices.AgregarLogARedis(logMessage);

            return Ok(new ResponseDto<PartidaDto>
            {
                Status = true,
                Message = "Partida eliminada correctamente.",
                Data = null
            });
        }
        catch (Exception)
        {
            // Log the exception
            return StatusCode(500, new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "Se produjo un error al eliminar la partida.",
                Data = null
            });
        }
    }
}