using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Persistencia.DapperConexion
{
    //Creo esta clase para poder codectarme dinamicamente a la BD mediante Dapper
    public class FactoryConnection : IFactoryConnection
    {
        private IDbConnection _connection;
        private readonly IOptions<ConexionConfiguracion> _configs;
        public FactoryConnection(IOptions<ConexionConfiguracion> configs)
        {
            _configs = configs;
        }

        public void CloseConnection() //Cierro la conexion
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
        public IDbConnection GetConnection() //Creo la conexion para Dapper
        {
            if (_connection == null) //Si no existe el objeto de conexion
            {
                _connection = new SqlConnection(_configs.Value.DefaultConnection); //Creo la cadena de conexion
            }
            if (_connection.State != ConnectionState.Open) //Si la cadena NO esta abierta
            {
                _connection.Open(); //lo abro
            }
            return _connection;
        }
    }
}
