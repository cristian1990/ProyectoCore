using Aplicacion.Contratos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Seguridad.TokenSeguridad
{
    //Es teoria la sesion porque JWT es stateless, y no se manejan sesiones, seria el usuario que hizo el login
    public class UsuarioSesion : IUsuarioSesion
    {
        //Inyecto IHttpContextAccessor, que sirve para saber que usuario hizo el login
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsuarioSesion(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ObtenerUsuarioSesion()
        {
            //Obtengo el userName del usuario actual (?) indico que puede ser null y no haber usuario logueado
            //Claims? : Si tiene un claims (es el nombreCompleto, o un grupo de rol) que me devuelva el valor de NameIdentifier
            var userName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userName;
        }
    }
}
