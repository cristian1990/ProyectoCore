using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Contratos
{
    //Es teoria la sesion porque JWT es stateless, y no se manejan sesiones, seria el usuario que hizo el login
    public interface IUsuarioSesion
    {
        string ObtenerUsuarioSesion();
    }
}
