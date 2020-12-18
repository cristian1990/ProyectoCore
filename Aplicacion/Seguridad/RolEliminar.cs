using Aplicacion.ManejadorError;
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
    //Clase utilizada para poder eliminar un Rol, mediante el nombre enviado desde el cliente
    public class RolEliminar
    {
        //Clase con el nombre de rol a eliminar
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; }
        }

        //Clase para validar la propiedad
        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.Nombre).NotEmpty(); //Regla para, no vacio
            }
        }

        //Clase que contiene la logica para poder eliminar un Rol
        public class Manejador : IRequestHandler<Ejecuta>
        {
            //Instancio e inyecto el IdentityRole, para eliminar un Rol
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Verificamos si existe el Rol que el cliente quiere eliminar
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                
                if (role == null) //Si no existe
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "No existe el rol" });
                }
                //Si existe... lo eliminamos
                var resultado = await _roleManager.DeleteAsync(role);
                
                if (resultado.Succeeded) //Si fue exitoso
                {
                    return Unit.Value; //Retornamos un flag (success)
                }

                throw new System.Exception("No se pudo eliminar el rol");
            }
        }
    }
}
