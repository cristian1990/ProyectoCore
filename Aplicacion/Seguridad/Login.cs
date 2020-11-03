using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    //En esta clase hacemos la logica de negocio
    public class Login
    {
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        //Clase para la validacion de datos, (lib: FluentValidation)
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Email).NotEmpty(); //Regla para, no vacio
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        //Clase de logica que verifica la existencia del usuario
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData> //Tipo de entidad a trabajar
        {

            private readonly UserManager<Usuario> _userManager;
            private readonly SignInManager<Usuario> _signInManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly CursosOnlineContext _context;

            //Hacemos la inyeccion de las dependencias
            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, CursosOnlineContext context, IJwtGenerador jwtGenerador)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jwtGenerador = jwtGenerador;
                _context = context;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Verificamos si el email existe
                var usuario = await _userManager.FindByEmailAsync(request.Email);
                if (usuario == null) //Si no existe
                {
                    //Lanzo una excepcion con el tipo de error que quiero que se envie
                    throw new ManejadorExcepcion(HttpStatusCode.Unauthorized); //401
                }

                //Si el email existe en la BD, hacemos el login, pasando el usuario y el password
                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);

                if (resultado.Succeeded) //Si fue exitoso
                {
                    //return usuario; //Retorno Todos los datos del usuario (No recomendable)

                    //Utilizo un DTO para retornar los datos que quiero
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CrearToken(usuario),
                        Username = usuario.UserName,
                        Email = usuario.Email,
                        Imagen = null
                    };
                }

                //Si no fue exitoso, lanzo una excepcion
                throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
            }
        }
    }
}
