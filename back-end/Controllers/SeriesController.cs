using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    public class SeriesController : ControllerBase
    {
        private readonly ApplicationDbcontext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "series";

        public SeriesController(ApplicationDbcontext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        // api/series
        [HttpGet]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 5;

            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Series
                .Where(x => x.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var netflix = await context.Series
                .Where(x => x.Plataforma == "Netflix")
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var hbo = await context.Series
                .Where(x => x.Plataforma == "HBO")
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var resultado = new LandingPageDTO();
            resultado.ProximosEstrenos = mapper.Map<List<SerieDTO>>(proximosEstrenos);
            resultado.Netflix = mapper.Map<List<SerieDTO>>(netflix);
            resultado.HBO = mapper.Map<List<SerieDTO>>(hbo);

            return resultado;

        }

        // api/series/1
        [HttpGet("{id}")]
        public async Task<ActionResult<SerieDTO>> Get(int id)
        {
            var serie = await context.Series
                .Include(x => x.SeriesGeneros).ThenInclude(x => x.Genero)
                .Include(x => x.SeriesActores).ThenInclude(x => x.Actor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (serie is null)
                return NotFound();

            var resultado = mapper.Map<SerieDTO>(serie);
            resultado.Actores = resultado.Actores.OrderBy(x => x.Orden).ToList();

            return resultado;
        }

        // api/series/postget
        [HttpGet("PostGet")]
        public async Task<ActionResult<SeriesPostGetDTO>> PostGet()
        {
            var generos = await context.Generos.ToListAsync();

            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return new SeriesPostGetDTO() { Generos = generosDTO };
        }

        // api/series/filtrar
        [HttpGet("filtrar")]
        public async Task<ActionResult<List<SerieDTO>>> Filtrar([FromQuery] SeriesFiltrarDTO seriesFiltrarDTO)
        {
            var seriesQueryable = context.Series.AsQueryable();

            if (!string.IsNullOrEmpty(seriesFiltrarDTO.Titulo))
            {
                seriesQueryable = seriesQueryable.Where(x => x.Titulo.Contains(seriesFiltrarDTO.Titulo));
            }

            if (seriesFiltrarDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                seriesQueryable = seriesQueryable.Where(x => x.FechaLanzamiento > hoy);
            }

            if (seriesFiltrarDTO.GeneroId != 0)
            {
                seriesQueryable = seriesQueryable
                    .Where(x => x.SeriesGeneros.Select(y => y.GeneroId)
                    .Contains(seriesFiltrarDTO.GeneroId));
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(seriesQueryable);

            var series = await seriesQueryable.Paginar(seriesFiltrarDTO.PaginacionDTO).ToListAsync();
            return mapper.Map<List<SerieDTO>>(series);

        }

        // api/series
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] SerieCreacionDTO serieCreacionDTO)
        {
            var serie = mapper.Map<Serie>(serieCreacionDTO);

            if (serieCreacionDTO.Imagen != null)
            {
                serie.Imagen = await almacenadorArchivos.GuardarArchivo(contenedor, serieCreacionDTO.Imagen);
            }

            EscribirOrdenActores(serie);

            context.Add(serie);
            await context.SaveChangesAsync();
            return serie.Id;
        }

        private void EscribirOrdenActores(Serie serie)
        {
            if (serie.SeriesActores != null)
            {
                for (int i = 0; i < serie.SeriesActores.Count; i++)
                {
                    serie.SeriesActores[i].Orden = i;
                }
            }

        }

        // api/series/putget/1

        [HttpGet("PutGet/{id}")]
        public async Task<ActionResult<SeriesPutGetDTO>> PutGet(int id)
        {
            var serieActionResult = await Get(id);
            if (serieActionResult.Result is NotFoundResult) { return NotFound(); }

            var serie = serieActionResult.Value;

            var generosSeleccionadosIds = serie.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos
                .Where(x => !generosSeleccionadosIds.Contains(x.Id))
                .ToListAsync();

            var generosNoSeleccionadosDTO = mapper.Map<List<GeneroDTO>>(generosNoSeleccionados);

            var respuesta = new SeriesPutGetDTO();
            respuesta.Serie = serie;
            respuesta.GenerosSeleccionados = serie.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionadosDTO;
            respuesta.Actores = serie.Actores;
            return respuesta;

        }

        // api/series/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] SerieCreacionDTO serieCreacionDTO)
        {
            var serie = await context.Series
                .Include(x => x.SeriesActores)
                .Include(x => x.SeriesGeneros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (serie is null)
                return NotFound();

            serie = mapper.Map(serieCreacionDTO, serie);

            if (serieCreacionDTO.Imagen != null)
            {
                serie.Imagen = await almacenadorArchivos.EditarArchivo(contenedor, serieCreacionDTO.Imagen, serie.Imagen);
            }

            EscribirOrdenActores(serie);

            await context.SaveChangesAsync();
            return NoContent();

        }

        // api/series/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var serie = await context.Series.FirstOrDefaultAsync(x => x.Id == id);

            if (serie is null)
                return NotFound();

            context.Remove(serie);
            await context.SaveChangesAsync();

            await almacenadorArchivos.BorrarArchivo(serie.Imagen, contenedor);

            return NoContent();
        }



    }
}