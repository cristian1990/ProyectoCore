using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Curso
    {
        //GUID: Global Unique IDentifier (Identificador único universal)
        public Guid CursoId { get; set; } //Es recomendable usar Guid para las Primary key
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; } // ?: Indico que permite null
        public byte[] FotoPortada { get; set; }

        //Tengo que agregar esta nueva propiedade tambien en el SP, para que la tome
        public DateTime? FechaCreacion { get; set; } // ?: Indico que permite null

        //PROPIEDADES DE NAVEGACION
        public Precio PrecioPromocion { get; set; }  //1 curso tiene un precio
        public ICollection<Comentario> ComentarioLista { get; set; } //1 curso puede tener muchos comentarios
        public ICollection<CursoInstructor> InstructoresLink { get; set; } //1 curso puede tener muchos instructores
    }
}
