using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    //Esta clase tiene la logica de buscar un curso por ID (Metodo GET)
    public class ConsultaId
    {
        //public class CursoUnico : IRequest<Curso>
        public class CursoUnico : IRequest<CursoDto>
        {
            //public int Id { get; set; }
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;

            //Hago la inyeccion de DbContext y del mapper
            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CursoDto> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                //Traigo el curso por ID y la data de los instructores, comentarios y precio
                var curso = await _context.Curso
                .Include(x => x.ComentarioLista)
                .Include(x => x.PrecioPromocion)
                .Include(x => x.InstructoresLink)
               .ThenInclude(y => y.Instructor)
               .FirstOrDefaultAsync(a => a.CursoId == request.Id);

                if (curso == null) //Si no lo encuentro
                {
                    //throw new Exception("No se encontro el curso");

                    //LLamo al manejador de excepciones, le paso el StatusCode y creo un nuevo objeto para el mensaje de error
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el ID del curso" });
                }

                //Hago el mapeo de entidad Curso a CursoDto
                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);

                return cursoDto;
            }
        }
    }
}
