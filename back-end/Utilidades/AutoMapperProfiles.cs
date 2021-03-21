using AutoMapper;
using SeriesAPI.DTOs;
using SeriesAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());

            CreateMap<SerieCreacionDTO, Serie>()
                .ForMember(x => x.Imagen, opciones => opciones.Ignore())
                .ForMember(x => x.SeriesGeneros, opciones => opciones.MapFrom(MapearSeriesGeneros))
                .ForMember(x => x.SeriesActores, opciones => opciones.MapFrom(MapearSeriesActores));

            CreateMap<Serie, SerieDTO>()
                .ForMember(x => x.Generos, opciones => opciones.MapFrom(MapearSeriesGeneros))
                .ForMember(x => x.Actores, opciones => opciones.MapFrom(MapearSeriesActores));
        }

        private List<SerieActorDTO> MapearSeriesActores(Serie serie, SerieDTO serieDTO)
        {
            var resultado = new List<SerieActorDTO>();

            if (serie.SeriesActores != null)
            {
                foreach (var actor in serie.SeriesActores)
                {
                    resultado.Add(new SerieActorDTO() 
                    { 
                        Id = actor.ActorId, 
                        Nombre = actor.Actor.Nombre,
                        Foto = actor.Actor.Foto,
                        Orden = actor.Orden,
                        Personaje = actor.Personaje
                        });
                }

            }

            return resultado;
        }
        private List<GeneroDTO> MapearSeriesGeneros(Serie serie, SerieDTO serieDTO)
        {
            var resultado = new List<GeneroDTO>();

            if(serie.SeriesGeneros != null){
                foreach (var genero in serie.SeriesGeneros)
                {
                    resultado.Add(new GeneroDTO() { Id = genero.GeneroId, Nombre = genero.Genero.Nombre });
                }

            }

            return resultado;
        }


        private List<SeriesActores> MapearSeriesActores(SerieCreacionDTO serieCreacionDTO, Serie serie)
        {
            var resultado = new List<SeriesActores>();

            if (serieCreacionDTO.Actores is null)
                return resultado;

            foreach (var actor in serieCreacionDTO.Actores)
            {
                resultado.Add(new SeriesActores() { ActorId = actor.Id, Personaje = actor.Personaje });
            }

            return resultado;
        }

        private List<SeriesGeneros> MapearSeriesGeneros(SerieCreacionDTO serieCreacionDTO, Serie serie)
        {
            var resultado = new List<SeriesGeneros>();

            if(serieCreacionDTO.GenerosIds is null)
                return resultado;

            foreach (var id in serieCreacionDTO.GenerosIds)
            {
                resultado.Add(new SeriesGeneros() { GeneroId = id });
            }

            return resultado;
        }
    }
}
