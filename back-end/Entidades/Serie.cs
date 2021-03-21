using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.Entidades
{
    public class Serie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:300)]
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public string Plataforma { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Imagen { get; set; }
        public List<SeriesActores> SeriesActores { get; set; }
        public List<SeriesGeneros> SeriesGeneros { get; set; }
    }
}
