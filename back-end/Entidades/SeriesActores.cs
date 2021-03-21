using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.Entidades
{
    public class SeriesActores
    {
        public int SerieId { get; set; }
        public int ActorId { get; set; }
        public Serie Serie { get; set; }
        public Actor Actor { get; set; }

        [StringLength(maximumLength: 100)]
        public string Personaje { get; set; }
        public int Orden { get; set; }
    }
}
