using System;
using System.Collections.Generic;
using System.Text;

namespace Persistencia.DapperConexion.Paginacion
{
    //Clase que contiene propiedades que son las que se van a retornar al cliente
    public class PaginacionModel
    {
        //Retorna un arreglo de tipo IDictionary (Retornara un JSON)
        public List<IDictionary<string, object>> ListaRecords { get; set; } //Cantidad de registros por pagina
        public int TotalRecords { get; set; } //Total de registros en la BD
        public int NumeroPaginas { get; set; } //Numero de paginas que se pueden crear
    }
}
