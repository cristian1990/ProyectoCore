using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : MiControllerBase
    {
        [HttpPost("crear")]
        public async Task<ActionResult<Unit>> Crear(RolNuevo.Ejecuta parametros) //Data que se envia del cliente
        {
            return await Mediator.Send(parametros); //Envio los parametros que envio el cliente
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<Unit>> Eliminar(RolEliminar.Ejecuta parametros) //Recibe el nombre de Rol a eliminar
        {
            return await Mediator.Send(parametros);
        }

        [HttpGet("lista")]
        public async Task<ActionResult<List<IdentityRole>>> Lista()
        {
            return await Mediator.Send(new RolLista.Ejecuta());
        }

        [HttpPost("agregarRoleUsuario")]
        public async Task<ActionResult<Unit>> AgregarRoleUsuario(UsuarioRolAgregar.Ejecuta parametros) //Parametros que envia el cliente
        {
            return await Mediator.Send(parametros);
        }

        [HttpDelete("eliminarRoleUsuario")]
        public async Task<ActionResult<Unit>> EliminarRoleUsuario(UsuarioRolEliminar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<List<string>>> ObtenerRolesPorUsuario(string username)
        {
            return await Mediator.Send(new ObtenerRolesPorUsuario.Ejecuta { Username = username });
        }
    }
}