using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.DTOs
{
    public class SeriesPutGetDTO
    {
        public SerieDTO Serie { get; set; }
        public List<GeneroDTO> GenerosSeleccionados { get; set; }

        public List<GeneroDTO> GenerosNoSeleccionados { get; set; }
        public List<SerieActorDTO> Actores { get; set; }
    }
}
