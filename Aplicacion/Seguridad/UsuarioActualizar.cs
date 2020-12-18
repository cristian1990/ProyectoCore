using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    //Esta clase sirve para actualizar los datos del usuario
    public class UsuarioActualizar
    {
        //Clase que contiene la data enviada desde el cliente
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; } //Necesario para saber que usuario quiere editar
            
            //public ImagenGeneral ImagenPerfil { get; set; }
        }

        //Clase para validar la data enviada por el cliente
        public class EjecutaValidador : AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.NombreCompleto).NotEmpty(); //Regla para, no vacio
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            //Inyeccion de dependencias necesarias para llevar a cabo los cambios en la entidad
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IPasswordHasher<Usuario> _passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _passwordHasher = passwordHasher;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco y obtengo el usuario, mediante su Username
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);
                if (usuarioIden == null) //Si NO existe
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe un usuario con este username" });
                }
                //Si existe... Verifico que el mail del usuario a actualizar no lo tenga otro usuario
                var resultado = await _context.Users.Where(x => x.Email == request.Email && x.UserName != request.Username).AnyAsync();
                if (resultado) //Si se encuentra el email
                {
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new { mensaje = "Este email pertenece a otro usuario" });
                }

                //Asigno los nuevos datos al usuario
                usuarioIden.NombreCompleto = request.NombreCompleto;
                usuarioIden.PasswordHash = _passwordHasher.HashPassword(usuarioIden, request.Password);
                usuarioIden.Email = request.Email;

                //Actualizo la data
                var resultadoUpdate = await _userManager.UpdateAsync(usuarioIden);

                //Obtenemos la lista de Roles del Usuario
                var resultadoRoles = await _userManager.GetRolesAsync(usuarioIden);
                var listRoles = new List<string>(resultadoRoles); //Casteo a una lista de string

                if (resultadoUpdate.Succeeded) //Si fue exitoso
                {
                    return new UsuarioData //Retornamos la data mediante el objeto UsuarioData
                    {
                        NombreCompleto = usuarioIden.NombreCompleto,
                        Username = usuarioIden.UserName,
                        Email = usuarioIden.Email,
                        Token = _jwtGenerador.CrearToken(usuarioIden, listRoles),
                        //ImagenPerfil = imagenGeneral
                    };
                }

                //Si no fue existoso...
                throw new System.Exception("No se pudo actualizar el usuario");
            }
        }
    }
}
