﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Catalogos;
using ProyectoApiContable.Dtos.EstadosPartidas;
using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Controllers;

[Route("api/EstadosPartidas")]
[ApiController]
public class EstadosPartidasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EstadosPartidasController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto<IReadOnlyList<EstadosPartidaDto>>>> ObtenerEstadosPartida()
    {
        var estadosPartidaDb = await _context.EstadosPartidas.ToListAsync();
        var estadosPartidaDto = _mapper.Map<List<EstadosPartidaDto>>(estadosPartidaDb);

        return Ok(new ResponseDto<List<EstadosPartidaDto>>
        {
            Status = true,
            Data = estadosPartidaDto
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto<EstadosPartidaDto>>> MostrarEstadoPartida(int id)
    {
        var estadoPartidaDb = await _context.EstadosPartidas.FindAsync(id);

        if (estadoPartidaDb == null)
        {
            var notFoundResponse = new ResponseDto<EstadosPartidaDto>
            {
                Status = false,
                Message = "Estado de Partida no encontrado.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        var estadoPartidaDto = _mapper.Map<EstadosPartidaDto>(estadoPartidaDb);

        var successResponse = new ResponseDto<EstadosPartidaDto>
        {
            Status = true,
            Message = "Estado de Partida recuperado correctamente.",
            Data = estadoPartidaDto
        };

        return Ok(successResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CrearEstadoPartida([FromBody] CreateEstadosPartidaDto createEstadoPartidaDto)
    {
        if (createEstadoPartidaDto == null)
        {
            var badRequestResponse = new ResponseDto<EstadosPartidaDto>
            {
                Status = false,
                Message = "Datos del Estado de Partida no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        // Mapear el CreateEstadoPartidaDto a una entidad EstadoPartida
        var estadoPartida = _mapper.Map<EstadoPartida>(createEstadoPartidaDto);

        // Guardar el Estado de Partida en la base de datos
        _context.EstadosPartidas.Add(estadoPartida);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con el Estado de Partida creado
        var estadoPartidaCreadoDto = _mapper.Map<EstadosPartidaDto>(estadoPartida);

        var successResponse = new ResponseDto<EstadosPartidaDto>
        {
            Status = true,
            Message = "Estado de Partida creado correctamente.",
            Data = estadoPartidaCreadoDto
        };

        return CreatedAtAction(nameof(MostrarEstadoPartida), new { id = estadoPartida.Id }, successResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEstadoPartida(int id,
        [FromBody] UpdateEstadosPartidaDto updateEstadoPartidaDto)
    {
        if (updateEstadoPartidaDto == null)
        {
            var badRequestResponse = new ResponseDto<EstadosPartidaDto>
            {
                Status = false,
                Message = "Datos del Estado de Partida no válidos.",
                Data = null
            };

            return BadRequest(badRequestResponse);
        }

        var estadoPartida = await _context.EstadosPartidas.FindAsync(id);

        if (estadoPartida == null)
        {
            var notFoundResponse = new ResponseDto<EstadosPartidaDto>
            {
                Status = false,
                Message = "Estado de Partida no encontrado.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        // Mapear el UpdateEstadoPartidaDto a la entidad EstadoPartida
        _mapper.Map(updateEstadoPartidaDto, estadoPartida);

        // Actualizar el Estado de Partida en la base de datos
        _context.EstadosPartidas.Update(estadoPartida);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con el Estado de Partida actualizado
        var estadoPartidaActualizadoDto = _mapper.Map<EstadosPartidaDto>(estadoPartida);

        var successResponse = new ResponseDto<EstadosPartidaDto>
        {
            Status = true,
            Message = "Estado de Partida actualizado correctamente.",
            Data = estadoPartidaActualizadoDto
        };

        return Ok(successResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEstadoPartida(int id)
    {
        var estadoPartida = await _context.EstadosPartidas.FindAsync(id);

        if (estadoPartida == null)
        {
            var notFoundResponse = new ResponseDto<EstadosPartidaDto>
            {
                Status = false,
                Message = "Estado de Partida no encontrado.",
                Data = null
            };

            return NotFound(notFoundResponse);
        }

        // Eliminar el Estado de Partida de la base de datos
        _context.EstadosPartidas.Remove(estadoPartida);
        await _context.SaveChangesAsync();

        // Retornar una respuesta con el Estado de Partida eliminado
        var estadoPartidaEliminadoDto = _mapper.Map<EstadosPartidaDto>(estadoPartida);

        var successResponse = new ResponseDto<EstadosPartidaDto>
        {
            Status = true,
            Message = "Estado de Partida eliminado correctamente.",
            Data = estadoPartidaEliminadoDto
        };

        return Ok(successResponse);
    }
}