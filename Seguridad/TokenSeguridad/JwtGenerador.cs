using Aplicacion.Contratos;
using Dominio;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Seguridad.TokenSeguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario/*, List<string> roles*/)
        {
            //Claims: es la data del usuario que quiero compartir con el cliente
            var claims = new List<Claim>{
                //Creo un nuevo Claim y agrego el usuario
                new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
            };

            //if (roles != null)
            //{
            //    foreach (var rol in roles)
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, rol));
            //    }
            //}

            //Creo las credenciales de acceso
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            //Paso la llave y algoritmo de encriptacion
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Creo la descripcion del Token
            var tokenDescripcion = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = credenciales
            };

            var tokenManejador = new JwtSecurityTokenHandler();
            var token = tokenManejador.CreateToken(tokenDescripcion); //Creo el token

            return tokenManejador.WriteToken(token); //Retorno el Token
        }
    }
}
