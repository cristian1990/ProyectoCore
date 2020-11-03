using System;
using System.Collections.Generic;
using System.Text;

namespace Persistencia.DapperConexion
{
    public class ConexionConfiguracion
    {
        //El valor de la conexion viene del proyecto WebAPI, de Startup
        public string DefaultConnection { get; set; }
    }
}
