using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.DTOs
{
    public class SerieDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public string Plataforma { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Imagen { get; set; }
        public List<GeneroDTO> Generos { get; set; }
        public List<SerieActorDTO> Actores { get; set; }
    }
}
