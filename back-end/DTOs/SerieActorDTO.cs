﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI.DTOs
{
    public class SerieActorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; }
    }
}
