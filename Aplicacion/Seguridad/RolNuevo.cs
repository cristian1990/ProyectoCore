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
    //Esta clase se encarga de manejar la logica, para crear un nuevo Rol en la base de datos mediante Identity Core
    public class RolNuevo
    {
        //Clase que recibe el parametro enviado por el cliente
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; } //Nombre del Rol
        }

        //Clase para validar el parametro que envia el cliente
        public class ValidaEjecuta : AbstractValidator<Ejecuta>
        {
            public ValidaEjecuta()
            {
                RuleFor(x => x.Nombre).NotEmpty(); //Regla para, no vacio
            }
        }

        //Clase que tiene la logica de agregar un Rol a la BD utilizando Identity Core
        public class Manejador : IRequestHandler<Ejecuta>
        {
            //Instancio e inyecto el IdentityRole, para crear un nuevo Rol
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco el Rol para verificar si existe
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                
                if (role != null) //Si existe el Rol
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "Ya existe el rol" });
                }
                //Si NO existe el Rol... Lo crea con el nombre proporcionado por el cliente
                var resultado = await _roleManager.CreateAsync(new IdentityRole(request.Nombre));
                
                if (resultado.Succeeded) //Si todo fue exitoso
                {
                    return Unit.Value; //retorno un Flag (success)
                }

                throw new Exception("No se pudo guardar el rol");
            }
        }
    }
}
