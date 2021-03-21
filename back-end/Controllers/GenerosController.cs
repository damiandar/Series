using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeriesAPI.DTOs;
using SeriesAPI.Entidades;
using SeriesAPI.Utilidades;

namespace SeriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbcontext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // api/generos
        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Generos.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var generos = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<GeneroDTO>>(generos);
        }

        // api/generos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if(genero is null)
                return NotFound();

            return mapper.Map<GeneroDTO>(genero);
        }

        // api/generos/todos
        [HttpGet("todos")]
        public async Task<ActionResult<List<GeneroDTO>>> Todos()
        {
            var generos = await context.Generos.ToListAsync();

            return mapper.Map<List<GeneroDTO>>(generos);
        }

        // api/generos
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Genero>(generoCreacionDTO);

            context.Add(genero);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // api/generos/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {

            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (genero is null)
                return NotFound();

            genero = mapper.Map(generoCreacionDTO, genero);

            await context.SaveChangesAsync();
            return NoContent();
        }

        // api/generos/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);

            if (!existe)
                return NotFound();

            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}