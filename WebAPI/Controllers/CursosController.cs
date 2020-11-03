using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Aplicacion.Cursos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI.Controllers
{
    //Este controller no se concta directamente Persistencia, si no al proyecto Aplicacion
    [Route("api/[controller]")]
    [ApiController]
    public class CursosController : MiControllerBase
    {
        //private readonly IMediator _mediator;

        //IMediator: Viene de la libreria MediatR, y lo usamos para conectarnos al 
        //proyecto Aplicacion Por eso inyectamos un objeto del tipo IMediator
        //Ya no hace falta porque hereda de MiControllerBase  
        //public CursosController(IMediator mediator)
        //{
        //    this._mediator = mediator;
        //}

        // https://localhost:44331/api/Cursos
        //Devuelvo al cliente la lista de cursos 
        [HttpGet]
        //[Authorize] Quito para hacerlo desde el startup
        public async Task<ActionResult<List<CursoDto>>> Get()
        {
            //Mediator, viene de MiControllerBase
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        // https://localhost:44331/api/Cursos/{id}
        // https://localhost:44331/api/Cursos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> Detalle(Guid id) //Cambio de int a Guid
        {
            return await Mediator.Send(new ConsultaId.CursoUnico { Id = id });
        }

        //Devuelve un Flag "Unit", para indicar si la transaccion se realizo correctamente, es de la lib MediatR
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, Editar.Ejecuta data)
        {
            data.CursoId = id;
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id) //Cambio de int a Guid
        {
            return await Mediator.Send(new Eliminar.Ejecuta { Id = id });
        }

        //Accion para la Paginacion, vendra data del cliente (Numero de Pagina, Cantidad de Elementos x pagina)
        [HttpPost("report")]
        public async Task<ActionResult<PaginacionModel>> Report(PaginacionCurso.Ejecuta data)
        {
            return await Mediator.Send(data);
        }
    }
}  