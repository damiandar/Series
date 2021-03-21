using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.DTOs
{
    public class LandingPageDTO
    {
        public List<SerieDTO> ProximosEstrenos { get; set; }
        public List<SerieDTO> Netflix { get; set; }
        public List<SerieDTO> HBO { get; set; }
    }
}
