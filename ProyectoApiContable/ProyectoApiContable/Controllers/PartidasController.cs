using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        if (createPartidaDto == null || createPartidaDto.FilasPartida == null || !createPartidaDto.FilasPartida.Any())
        {
            var badRequestResponse = new ResponseDto<PartidaDto>
            {
                Status = false,
                Message = "Datos de la partida o filas de partida no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        // Mapear el CreatePartidaDto a una entidad Partida
        var partidaTemporal = _mapper.Map<Partida>(createPartidaDto);

        // Puedes establecer propiedades adicionales antes de guardar, por ejemplo, la fecha de creación
        partidaTemporal.FechaCreacion = DateTime.Now;

        // Guardar temporalmente la partida en la base de datos para obtener un ID temporal
        _context.Partidas.Add(partidaTemporal);
        await _context.SaveChangesAsync();

        // Mapear y asociar las FilasPartida al ID temporal de la Partida
        var filasPartida = _mapper.Map<List<FilasPartida>>(createPartidaDto.FilasPartida);
        filasPartida.ForEach(fp => fp.PartidaId = partidaTemporal.Id);

        // Guardar temporalmente las FilasPartida en la base de datos
        _context.FilasPartidas.AddRange(filasPartida);
        await _context.SaveChangesAsync();

        // Actualizar las FilasPartida con el ID definitivo de la Partida
        var partidaDefinitiva = await _context.Partidas.FindAsync(partidaTemporal.Id);
        var filasPartidaDefinitivas = await _context.FilasPartidas
            .Where(fp => fp.PartidaId == partidaTemporal.Id)
            .ToListAsync();

        filasPartidaDefinitivas.ForEach(fp => fp.PartidaId = partidaDefinitiva.Id);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con la partida creada
        var partidaCreadaDto = _mapper.Map<PartidaDto>(partidaDefinitiva);

        var successResponse = new ResponseDto<PartidaDto>
        {
            Status = true,
            Message = "Partida y filas de partida creadas correctamente.",
            Data = partidaCreadaDto
        };

        return CreatedAtAction(nameof(MostrarPartida), new { id = partidaDefinitiva.Id }, successResponse);
    }

}

