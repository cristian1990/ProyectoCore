using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] //Indico que se puede acceder a os metodos sin hacer login (salto la conf de Startup)
    public class UsuarioController : MiControllerBase
    {
        // https://localhost:44331/api/Usuario/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioData>> Login(Login.Ejecuta parametros) 
        {
            //Envio los parametros, Email y password. Retorno un UsuarioData
            return await Mediator.Send(parametros); //Mediator viene de MiControllerBase
        }

        // https://localhost:44331/api/Usuario/registrar
        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioData>> Registrar(Registrar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //// https://localhost:44331/api/Usuario
        [HttpGet]
        public async Task<ActionResult<UsuarioData>> DevolverUsuario()
        {
            //Retorno el usuario actual en logueado
            return await Mediator.Send(new UsuarioActual.Ejecutar());
        }

        [HttpPut]
        public async Task<ActionResult<UsuarioData>> Actualizar(UsuarioActualizar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }
    }
}