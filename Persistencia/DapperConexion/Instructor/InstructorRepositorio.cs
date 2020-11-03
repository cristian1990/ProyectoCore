using Dapper;
using Persistencia.DapperConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;

        public InstructorRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }

        //Logica para conseguir la data del Stored Procedure
        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList = null;

            //Nombre del stored procedure que va a utilizar y que se encuentra en la Base de datos
            var storeProcedure = "usp_Obtener_Instructores";
            
            try
            {
                //Llamo al objeto de conexion
                var connection = _factoryConnection.GetConnection();
                //Obtengo la lista de instructores con tipo InstructorModel.
                //QueryAsync: devuelve un tipo IEnumerable, por eso indico que la lista instructorList es de ese tipo
                //null: porque el storeProcedure no recibe ningun parametro
                //commandType: Indico que tipon de transaccion es, en este caso StoredProcedure
                instructorList = await connection.QueryAsync<InstructorModel>( //Mapeo a InstructorModel
                    storeProcedure, //Nombre del SP
                    null, //Sin parametros que enviar al SP
                    commandType: CommandType.StoredProcedure); //Tipo de comando
            }
            catch (Exception e)
            {
                throw new Exception("Error en la consulta de datos", e);
            }
            finally
            {
                //Cerramos siempre la conexion, haya resultado exitosa la consulta o no
                _factoryConnection.CloseConnection();
            }
            return instructorList;
        }

        public async Task<int> Nuevo(string nombre, string apellidos, string titulo)
        {
            //Nombre del stored procedure que va a utilizar y que se encuentra en la Base de datos
            var storeProcedure = "usp_instructor_nuevo";

            try
            {
                var connection = _factoryConnection.GetConnection();
                //ExecuteAsync: Recibe 3 parametros. (El stored Procedure, Parametros a ingresar en el SP, y el tipo de dato)
                //Devuelve un valor de tipo entero, indicando cuantas transacciones se realizaron correctamente al llamar al SP
                var resultado = await connection.ExecuteAsync(
                storeProcedure,
                new //Creo un nuevo instructor con datos enviado del cliente
                {
                    InstructorId = Guid.NewGuid(), //Creo in Id aleatorio
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Titulo = titulo
                },
                commandType: CommandType.StoredProcedure
                );

                //Cerramos la conxion
                _factoryConnection.CloseConnection();

                //Retornamos el resultado
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo guardar el nuevo instructor", e);
            }
        }

        public async Task<int> Actualiza(Guid instructorId, string nombre, string apellidos, string titulo)
        {
            //Nombre del stored procedure que va a utilizar y que se encuentra en la Base de datos
            var storeProcedure = "usp_instructor_editar";

            try
            {
                var connection = _factoryConnection.GetConnection(); //Obtengo el objeto de conexion
                //ExecuteAsync: Recibe 3 parametros. (El stored Procedure, Parametros a ingresar en el SP, y el tipo de dato)
                //Devuelve un valor de tipo entero, indicando cuantas transacciones se realizaron correctamente al llamar al SP
                var resultados = await connection.ExecuteAsync(
                    storeProcedure,
                    new //Creo un nuevo instructor con datos enviado del cliente
                    {
                        InstructorId = instructorId,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Titulo = titulo
                    },
                    commandType: CommandType.StoredProcedure
                );

                _factoryConnection.CloseConnection(); //Cierro la conexion
                return resultados; //Retorno el numero de registros afectados

            }
            catch (Exception e)
            {
                throw new Exception("No se pudo editar la data del instructor", e);
            }
        }

        public async Task<int> Elimina(Guid id)
        {
            //Nombre del stored procedure que va a utilizar y que se encuentra en la Base de datos
            var storeProcedure = "usp_instructor_elimina";

            try
            {
                var connection = _factoryConnection.GetConnection(); //Habro la conexion
                var resultado = await connection.ExecuteAsync( //Asigno los 3 parametros
                    storeProcedure,
                    new
                    {
                        InstructorId = id
                    },
                    commandType: CommandType.StoredProcedure
                );

                _factoryConnection.CloseConnection(); //Cierro la conexion
                return resultado; //Retorno el numero de registros afectados
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo eliminar el instructor", e);
            }
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            //Nombre del stored procedure que va a utilizar y que se encuentra en la Base de datos
            var storeProcedure = "usp_obtener_instructor_por_id";
           
            InstructorModel instructor = null;

            try
            {
                var connection = _factoryConnection.GetConnection(); //Abro la conexion
                //QueryFirstAsync: Asigno los 3 parametros necesario y obtengo el instructor por ID
                instructor = await connection.QueryFirstAsync<InstructorModel>( //Mapeo a InstructorModel
                    storeProcedure, //Nombre del SP
                    new
                    {
                        Id = id //Parametro del SP
                    },
                    commandType: CommandType.StoredProcedure //Tipo de comando
                );

                return instructor; //Retorno el instructor

            }
            catch (Exception e)
            {
                throw new Exception("No se pudo encontrar el instructor", e);
            }
        }
    }
}
