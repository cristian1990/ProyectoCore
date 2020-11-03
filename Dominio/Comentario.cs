using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Comentario
    {
        //GUID: Global Unique IDentifier (Identificador único universal)
        //public int ComentarioId { get; set; }
        public Guid ComentarioId { get; set; } //Es recomendable usar Guid para las Primary key
        public string Alumno { get; set; }
        public int Puntaje { get; set; }
        public string ComentarioTexto { get; set; }
        public Guid CursoId { get; set; } //Cambio de int a Guid

        //Tengo que agregar esta nueva propiedade tambien en el SP, para que la tome
        public DateTime? FechaCreacion { get; set; } // ?: Permite null

        //PROPIEDADES DE NAVEGACION
        public Curso Curso { get; set; } //1 comentario le pertenece solo a un curso
    }
}
