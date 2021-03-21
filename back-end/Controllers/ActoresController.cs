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
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbcontext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        // api/actores
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<ActorDTO>>(actores);
        }

        // api/actores/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
                return NotFound();

            return mapper.Map<ActorDTO>(actor);
        }

        // api/actores
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto);
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // api/actores/buscarPorNombre
        [HttpPost("buscarPorNombre")]
        public async Task<ActionResult<List<SerieActorDTO>>> BuscarPorNombre([FromBody] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<SerieActorDTO>();

            return await context.Actores
                .Where(x => x.Nombre.Contains(nombre))
                .Select(x => new SerieActorDTO { Id = x.Id, Nombre = x.Nombre, Foto = x.Foto })
                .Take(5)
                .ToListAsync();
        }

        // api/actores/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {

            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
                return NotFound();

            actor = mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.EditarArchivo(contenedor, actorCreacionDTO.Foto, actor.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        // api/actores/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
                return NotFound();

            context.Remove(actor);
            await context.SaveChangesAsync();

            await almacenadorArchivos.BorrarArchivo(actor.Foto, contenedor);

            return NoContent();
        }

    }
}