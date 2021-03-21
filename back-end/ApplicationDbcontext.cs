using Microsoft.EntityFrameworkCore;
using SeriesAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesAPI
{
    public class ApplicationDbcontext : DbContext
    {
        public ApplicationDbcontext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeriesActores>()
                .HasKey(x => new { x.ActorId, x.SerieId });

            modelBuilder.Entity<SeriesGeneros>()
                .HasKey(x => new { x.SerieId , x.GeneroId});

            base.OnModelCreating(modelBuilder); 
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Serie> Series { get; set; }
        public DbSet<SeriesActores> SeriesActores { get; set; }
        public DbSet<SeriesGeneros> SeriesGeneros { get; set; }
    }
}
