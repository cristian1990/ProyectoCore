using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            //Hago la inyeccion del DbContext
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //==== ELIMINO LOS INSTRUCTORES DEL CURSO
                //Obtengo toda la lista de instructores referentes al curso
                var instructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.Id);

                foreach (var instructor in instructoresDB)
                {
                    //Itero y voy eliminando los instructores del curso 
                    _context.CursoInstructor.Remove(instructor);
                }
                //====

                //==== ELIMINO LOS COMENTARIOS DEL CURSO
                //Obtengo toda la lista de comentarios referentes al curso
                var comentariosDB = _context.Comentario.Where(x => x.CursoId == request.Id);
                foreach (var cmt in comentariosDB)
                {
                    //Itero y voy eliminando los comentarios del curso 
                    _context.Comentario.Remove(cmt);
                }
                //====

                //==== ELIMINO EL PRECIO DEL CURSO
                //Obtengo el precio referente al curso
                var precioDB = _context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();
                if (precioDB != null)
                {
                    //Elimino el precio del curso
                    _context.Precio.Remove(precioDB);
                }
                //====

                //==== ELIMINO EL CURSO MEDIANTE SU ID
                var curso = await _context.Curso.FindAsync(request.Id); //Busco el curso por Id
               
                if (curso == null) //Si no lo encuentro
                {
                    //throw new Exception("No se encontro el curso");

                    //LLamo al manejador de excepciones, le paso el StatusCode y creo un nuevo objeto para el mensaje de error
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el curso" });
                }

                _context.Remove(curso); //Si lo encontro, indico que quiero eliminar
                //====

                var resultado = await _context.SaveChangesAsync(); //Elimino el Curso de la BD

                if (resultado > 0) //Si es mayor a 0 quiere decir que se elimino correctamente
                {
                    return Unit.Value; //Retorno un Flag
                }

                //Si no se pudo eliminar, retorno una Excepcion
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}
