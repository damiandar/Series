using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeriesAPI.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.DTOs
{
    public class SerieCreacionDTO
    {
        [Required]
        [StringLength(maximumLength: 300)]
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public string Plataforma { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile Imagen { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorSerieCreacionDTO>>))]
        public List<ActorSerieCreacionDTO> Actores { get; set; }
    }
}
