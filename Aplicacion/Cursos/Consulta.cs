using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    //Esta clase se encarga de la logica de devolver la lista de cursos dentro de la BD
    public class Consulta
    {
        public class ListaCursos : IRequest<List<CursoDto>> { } //IRequest: Propio de la lib MadiatR

        //IRequestHandler<QueQuieroQueDevuelva, FormatoEnElQueDevuelve>
        public class Manejador : IRequestHandler<ListaCursos, List<CursoDto>>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;
            public Manejador(CursosOnlineContext context ,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                //Obtengo la lista de cursos y los datos de sus instructores, comentarios y precio
                var cursos = await _context.Curso
                    .Include(x => x.ComentarioLista)
                    .Include(x => x.PrecioPromocion)
                    .Include(x => x.InstructoresLink)
                    .ThenInclude(x => x.Instructor)
                    .ToListAsync();

                //Hago el mapeo de List Curso hacia List CursoDto
                var cursosDto = _mapper.Map<List<Curso>, List<CursoDto>>(cursos);

                return cursosDto;
            }
        }
    }

}
