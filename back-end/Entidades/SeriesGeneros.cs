using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.Entidades
{
    public class SeriesGeneros
    {
        public int SerieId { get; set; }
        public int GeneroId { get; set; }
        public Serie Serie { get; set; }
        public Genero Genero { get; set; }
    }
}
