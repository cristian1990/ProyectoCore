using System;
using System.Collections.Generic;
using System.Text;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorModel
    {
        //Indico la data que voy a retornar de la BD
        public Guid InstructorId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        
        //Modifico el nombre de Grado a Titulo, y eso tambien lo hago en el Stored Procedure mediante un Alias
        public string Titulo { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
