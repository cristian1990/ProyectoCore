using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistencia
{
    //public class CursosOnlineContext : DbContext
    public class CursosOnlineContext : IdentityDbContext<Usuario> //Cambio ya que voy a usar Identity
    {
        public CursosOnlineContext(DbContextOptions options) : base(options) //Utiliza el option del DbContext
        { }

        //==== API Fluent ====
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Definimos que la entidad CursoInstructor tiene una clave primaria compuesta, tiene 2 columnas que representan una Primary Key
            modelBuilder.Entity<CursoInstructor>().HasKey(ci => new { ci.InstructorId, ci.CursoId });

            //Agrego el SeedData
            //SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        //==== Ingresamos Data de inicio ====
        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    var usuario = new Usuario { NombreCompleto = "Cristian Silva", UserName = "Crismo", Email = "cristian@gmail.com"};

        //    //modelBuilder.Entity<Usuario>().HasData(usuario);
        //}

        //==== DbSet ====
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<CursoInstructor> CursoInstructor { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Precio> Precio { get; set; }
    }
}
