using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
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
    public class UsuarioRolEliminar
    {
        //Clase de data que envia el cliente
        public class Ejecuta : IRequest
        {
            public string Username { get; set; } //Usuario del que se quiere eliminar el Rol
            public string RolNombre { get; set; } //Nombre del Rol a eliminar
        }

        //Clase para validar la data enviada por el cliente
        public class EjecutaValidador : AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.Username).NotEmpty(); //Regla para, no vacio
                RuleFor(x => x.RolNombre).NotEmpty();
            }
        }

        //Clase que se encarga de la logica para eliminar un Rol de un Usuario 
        public class Manejador : IRequestHandler<Ejecuta>
        {
            //Instancio y hago la Inyeccion de las clases necesarias para trabajar con estas entidades de Identity Core
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco el nombre del Rol 
                var role = await _roleManager.FindByNameAsync(request.RolNombre);             
                if (role == null) //Si NO existe
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el rol" });
                }

                //Si existe... Busco y obtengo el ID(Guid) del usuario de la BD  
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);      
                if (usuarioIden == null) //Si NO existe
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el usuario" });
                }

                //Si esxiste el Rol y el Usuario...  Mediante el ID del usuario, elimino el Rol al usuario indicado
                var resultado = await _userManager.RemoveFromRoleAsync(usuarioIden, request.RolNombre);
                if (resultado.Succeeded) //Si es exitoso
                {
                    return Unit.Value; //Retorno un Flag (success)
                }

                throw new Exception("No se pudo eliminar el rol");
            }
        }
    }
}
