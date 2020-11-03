using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Dominio
{
    public class Precio
    {
        public Guid PrecioId { get; set; } //Es recomendable usar Guid para las Primary key

        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioActual { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Promocion { get; set; }
        public Guid CursoId { get; set; }

        //PROPIEDADES DE NAVEGACION
        public Curso Curso { get; set; }
    }
}
