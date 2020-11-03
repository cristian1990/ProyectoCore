using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public interface IPaginacion
    {
        //Metodo asincrono que representa la paginacion, retorna un PaginacionModel
        //Asigno los parametros necesarios para la paginacion
        Task<PaginacionModel> devolverPaginacion(
            string storeProcedure,
            int numeroPagina,
            int cantidadElementos,
            IDictionary<string, object> parametrosFitro, //Pueden ser multiples
            string ordenamientoColumna
            );
    }
}
