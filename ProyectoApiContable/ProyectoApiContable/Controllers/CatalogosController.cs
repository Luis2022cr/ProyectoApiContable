using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Catalogos;
using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Controllers
{ 
    [Route("api/Catalogos")]
    [ApiController]
    public class CatalogosController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CatalogosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<CatalogoDto>>>> Get()
        {
            var catalogosDb = await _context.CatalogoCuentas.ToListAsync();
            var catalogosDto = _mapper.Map<List<CatalogoDto>>(catalogosDb);
            return Ok(new ResponseDto<List<CatalogoDto>>
            {
                Status = true,
                Data = catalogosDto
            });
        }
        [HttpGet("{id:int}")]
        //[Authorize]

        public async Task<ActionResult<CatalogoGetByIdDto>> GetOneById(int id)
        {

            var catalogoDb = await _context.CatalogoCuentas.FirstOrDefaultAsync(x => x.Id == id);

            var catalogoDto = _mapper.Map<CatalogoGetByIdDto>(catalogoDb);

            if (catalogoDb is null)
            {
                return NotFound(new ResponseDto<CatalogoGetByIdDto>
                {
                    Status = false,
                    Message = $"El catalogo con id {id}, no fue encontrado."
                });
            }

            catalogoDto = _mapper.Map<CatalogoGetByIdDto>(catalogoDb);

            return Ok(new ResponseDto<CatalogoGetByIdDto>
            {
                Status = true,
                Data = catalogoDto
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<CatalogoDto>>> Post(CatalogoCreateDto dto)
        {

            var catalogo = _mapper.Map<CatalogoCuentas>(dto);

            _context.Add(catalogo);
            await _context.SaveChangesAsync();

            var catalogoDto = _mapper.Map<Dtos.Catalogos.CatalogoDto>(catalogo);

            return StatusCode(StatusCodes.Status201Created, new ResponseDto<CatalogoDto>
            {
                Status = true,
                Data = catalogoDto
            });
        }

        [HttpPut("{id:int}")] // api/catalogos/4
        public async Task<ActionResult<ResponseDto<CatalogoGetByIdDto>>> Put(int id, CatalogoUpdateDto dto)
        {
            var catalogoDb = await _context.CatalogoCuentas.FirstOrDefaultAsync(a => a.Id == id);

            if (catalogoDb is null)
            {
                return NotFound(new ResponseDto<CatalogoGetByIdDto>
                {
                    Status = false,
                    Message = $"El catalogo con id {id}, no fue encontrado."
                });
            }

            _mapper.Map<CatalogoUpdateDto, CatalogoCuentas>(dto, catalogoDb);

            _context.Update(catalogoDb);
            await _context.SaveChangesAsync();

            var catalogoDto = _mapper.Map<CatalogoGetByIdDto>(catalogoDb);

            return Ok(new ResponseDto<CatalogoGetByIdDto>
            {
                Status = true,
                Message = "Catalogo editado correctamente",
                Data = catalogoDto
            });
        } 

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto<string>>> Delete(int id)
        {
            var catalogo = await _context.CatalogoCuentas.FirstOrDefaultAsync(a => a.Id == id);
            if (catalogo is null)
            {
                return NotFound(new ResponseDto<CatalogoGetByIdDto>
                {
                    Status = false,
                    Message = $"El catalogo con id {id}, no fue encontrado."
                });
            }

            _context.Remove(catalogo);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<string>
            {
                Status = true,
                Message = $"El catalogo con el id {id} fue borrado"
            });
        }
    }
}
