using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class CursoInstructor
    {
        public Guid CursoId { get; set; } //Es recomendable usar Guid para las Primary key
        public Guid InstructorId { get; set; }


        //PROPIEDADES DE NAVEGACION
        public Curso Curso { get; set; } 
        public Instructor Instructor { get; set; }
    }
}
