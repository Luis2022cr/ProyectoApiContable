using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Partidas;
using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Controllers;

[Route("api/partidas")]
[ApiController]


public class PartidasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PartidasController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
     [HttpGet]
    public IActionResult MostrarTodasLasPartidas()
    {
        var partidas = _context.Partidas.ToList();
        var partidasDto = _mapper.Map<List<PartidaDto>>(partidas);

        var response = new ResponseDto<List<PartidaDto>>
        {
            Status = true,
            Message = "Partidas recuperadas correctamente.",
            Data = partidasDto
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> MostrarPartida(Guid id)
    {
        var partida = await _context.Partidas.FindAsync(id);

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

    [HttpPost]
    public async Task<IActionResult> CrearPartida([FromBody] CreatePartidaDto createPartidaDto)
    {
        if (createPartidaDto == null || !ModelState.IsValid)
        {
            // Manejar la solicitud no válida
            var badRequestResponse = new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "Datos de la partida no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        // Mapear el CreatePartidaDto a la entidad Partida
        var partida = _mapper.Map<Partida>(createPartidaDto);

        // Configurar otros campos de la partida si es necesario
        partida.FechaCreacion = DateTime.Now;
    
        // Asignar el EstadoPartidaId por defecto (en este caso, 2)
        partida.EstadoPartidaId = 2;

        // // Obtener el usuario actualmente autenticado
        var usuarioActual = "hola"; //await _userManager.GetUserAsync(User);

        if (usuarioActual != null)
        {
            // Asignar el email del usuario como CreadoPor
            partida.CreadoPorId = usuarioActual;
        }
        else
        {
          
            var userNotFoundResponse = new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "No se pudo encontrar el usuario actual.",
                Data = null
            };

            return BadRequest(userNotFoundResponse);
        }

        // Guardar la partida en la base de datos
        _context.Partidas.Add(partida);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con la partida creada
        var partidaCreadaDto = _mapper.Map<PartidaDto>(partida);

        var successResponse = new ResponseDto<PartidaDto>
        {
            Status = true,
            Message = "Partida creada correctamente.",
            Data = partidaCreadaDto
        };

        return CreatedAtAction(nameof(MostrarPartida), new { id = partida.Id }, successResponse);
    }


}

