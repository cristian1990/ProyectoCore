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
    //Clase para asignar un Rol a un Usuario
    public class UsuarioRolAgregar
    {
        //Clase que contiene la data enviada por el cliente
        public class Ejecuta : IRequest
        {
            public string Username { get; set; } //Usuario a asignar un rol
            public string RolNombre { get; set; } //Rol que se debe asignar a dicho usuario
        }

        //Esta clase valida la data enviada del cliente
        public class EjecutaValidador : AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.Username).NotEmpty(); //Regla para, no vacio
                RuleFor(x => x.RolNombre).NotEmpty();
            }
        }

        //Clase que contiene la logica para asignar un Rol a un Usuario
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
                //Verifico si esxiste el Rol en la BD
                var role = await _roleManager.FindByNameAsync(request.RolNombre);            
                if (role == null) //Si NO existe el Rol
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "El rol no existe" });
                }

                //Si existe... Busco y obtengo el ID(Guid) del usuario de la BD  
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);             
                if (usuarioIden == null) //Si NO existe el usuario
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "El usuario no existe" });
                }

                //Si existe el Rol y el Usuario... Mediante el ID del usuario asigno el Rol indicado
                var resultado = await _userManager.AddToRoleAsync(usuarioIden, request.RolNombre);
                if (resultado.Succeeded) //Si es exitoso
                {
                    return Unit.Value; //Retorno un Flag (success)
                }

                throw new Exception("No se pudo agregar el Rol al usuario");
            }
        }
    }
}
