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
    //Esta clase sirve para registrar un nuevo usuario
    public class Registrar
    {
        //Propiedades de la entidad
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
        }

        //Clase para manejar las validaciones
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

        //Clase para la logica del registro de usuarios en la BD
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;

            //Inyeccion de las dependencias
            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
            }


            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Verificamos si el Email del usuario existe
                var existe = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();
                if (existe)
                {
                    //Respondo con un 400, y envio un mje de error
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "Ya existe un usuario registrado con este email" });
                }

                //Verificamos si el UserName del usuario existe
                var existeUserName = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();
                if (existeUserName)
                {
                    //Respondo con un 400, y envio un mje de error
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "Ya existe un usuario con este username" });
                }

                //Si no existe el Email ni el UserName, creo una nueva instancia de usuario con los datos enviados
                var usuario = new Usuario
                {
                    NombreCompleto = request.NombreCompleto,
                    Email = request.Email,
                    UserName = request.Username
                };

                //Creo el usuario con la pass
                var resultado = await _userManager.CreateAsync(usuario, request.Password);

                if (resultado.Succeeded) //Si fue exitoso
                {
                    //Retorno un UsuarioData
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CrearToken(usuario),
                        Username = usuario.UserName,
                        Email = usuario.Email
                    };
                }

                throw new Exception("No se pudo agregar al nuevo usuario");
            }
        }
    }
}
