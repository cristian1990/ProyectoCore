using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Instructor
    {
        public Guid InstructorId { get; set; } //Es recomendable usar Guid para las Primary key
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Grado { get; set; }
        public byte[] FotoPerfil { get; set; }
        
        //Tengo que agregar esta nueva propiedade tambien en el SP, para que la tome
        public DateTime? FechaCreacion { get; set; } // ?: Indico que permite null

        //PROPIEDADES DE NAVEGACION
        public ICollection<CursoInstructor> CursoLink { get; set; } 
    }
}
