using Aplicacion.ManejadorError;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    //Esta clase sirve para obtener los Roles que tiene un usuario
    public class ObtenerRolesPorUsuario
    {
        //Clase de contiene la data que envia el usuario
        public class Ejecuta : IRequest<List<string>> //Indico que retorna una lista de string
        {
            public string Username { get; set; } //Usuario del que se pide la lista de roles asignados
        }

        //Clase que contiene la logica para buscar la lista de Roles de algun Usuario
        public class Manejador : IRequestHandler<Ejecuta, List<string>>
        {
            //Instancio y hago la Inyeccion de las clases necesarias para trabajar con estas entidades de Identity Core
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
                _userManager = userManager;
            }

            public async Task<List<string>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco y obtengo el ID(Guid) del usuario en la BD
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);
                if (usuarioIden == null) //Si no existe en usuario
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe el usuario" });
                }
                //Si existe... Obtengo los roles del Usuario indicado
                var resultados = await _userManager.GetRolesAsync(usuarioIden);

                //Casteo de IList a List y retorno la lista de Roles
                return new List<string>(resultados);  
            }
        }
    }
}
