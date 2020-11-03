using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepositorio : IPaginacion
    {
        //Inyecto la conexion de Dapper
        private readonly IFactoryConnection _factoryConnection;
        public PaginacionRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }

        //Logica para realizar la paginacion
        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFitro, string ordenamientoColumna)
        {
            PaginacionModel paginacionModel = new PaginacionModel(); //Instancio el obj a retornar
            List<IDictionary<string, object>> listaReporte = null; //Se va a llenar con la data que viene de la BD
            int totalRecords = 0;
            int totalPaginas = 0;
           
            try
            {
                var connection = _factoryConnection.GetConnection(); //Abro la conexion
                //Creo un objeto para representar los parametros que voy a pasar al SP
                DynamicParameters parametros = new DynamicParameters();

                //Va a leer los datos del arreglo parametrosFitro
                foreach (var param in parametrosFitro)
                {
                    //Si tiene valores se va a convertir dinamicamente a un parametrosFitro
                    parametros.Add("@" + param.Key, param.Value);
                }

                //Parametros de entrada que se envian al SP para realizar la operacion
                parametros.Add("@NumeroPagina", numeroPagina);
                parametros.Add("@CantidadElementos", cantidadElementos);
                parametros.Add("@Ordenamiento", ordenamientoColumna);

                //Parametros de salida que devuelve el SP 
                parametros.Add("@TotalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parametros.Add("@TotalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);

                //Ejecuto el Stored Procedure, pasando los parametros necesarios
                //storeProcedure: Nombre del SP en la base de datos
                //parametros: Parametros que se recibe el SP
                //commandType: Tipo de comando
                var result = await connection.QueryAsync(storeProcedure, parametros, commandType: CommandType.StoredProcedure);

                //Convierto cada registro de la data que viene de la BD de IEnumerable a Dictionary
                listaReporte = result.Select(reg => (IDictionary<string, object>)reg).ToList();
                
                //Asigno los valores a las propiedades
                paginacionModel.ListaRecords = listaReporte;
                paginacionModel.NumeroPaginas = parametros.Get<int>("@TotalPaginas"); //Asigno el valor devuelto por el SP
                paginacionModel.TotalRecords = parametros.Get<int>("@TotalRecords"); //Asigno el valor devuelto por el SP
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo ejecutar el procedimiento almacenado", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }

            return paginacionModel;
        }
    }
}
