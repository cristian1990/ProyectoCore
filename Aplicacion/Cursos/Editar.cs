using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
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
    public class Editar
    {
        public class Ejecuta : IRequest
        {
            public Guid CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; } // ?: Indica que puede ser null    
            public List<Guid> ListaInstructor { get; set; }
            public decimal? Precio { get; set; }
            public decimal? Promocion { get; set; }
        }


        //Creo una nueva clase para la logica de la validacion, para esto utilice la lib FluentValidation
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty(); //Regla para Titulo, No vacio
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //==== CURSO A EDITAR
                //Buscamos el curso a editar, mediante su ID
                var curso = await _context.Curso.FindAsync(request.CursoId);
                if (curso == null) //Si no lo encuentro
                {
                    //throw new Exception("No se encontro el curso");

                    //LLamo al manejador de excepciones, le paso el StatusCode y creo un nuevo objeto para el mensaje de error
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el curso" });
                }
                //====

                //==== INGRESAMOS LA NUEVA DATA AL CURSO
                //Si no envio un titulo para modificar, va a mantener el actual. 
                //El operador ??, evalua si es null request.Titulo y si es asi, mantiene el valor anterior
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow; //Asigno el tiempo actual con Utc para evitar problema con las ubicaciones los clientes
                //====

                //==== ACTUALIZAR PRECIO DEL CURSO
                //Busco el curso a editar mediante su ID
                var precioEntidad = _context.Precio.Where(x => x.CursoId == curso.CursoId).FirstOrDefault();
                if (precioEntidad != null) //Si tiene un precio asignado
                {
                    //Si el valor enviado es null se mantiene el anterior
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                }
                else //Si no es null, lo insertamos en la BD
                {
                    precioEntidad = new Precio
                    {
                        PrecioId = Guid.NewGuid(), //Asignamos un Id aleatorio con formato Guid 
                        PrecioActual = request.Precio ?? 0, //Si el precio enviado es null, asignamos 0
                        Promocion = request.Promocion ?? 0,
                        CursoId = curso.CursoId //Asignamos el Id del curso 
                    };
                    await _context.Precio.AddAsync(precioEntidad);
                }
                //====

                //==== ACTUALIZAR DATA DE INSTRUCTORES    
                //Priero elimino la lista de instructores y despues agrego de nuevo
                if (request.ListaInstructor != null)  //Si el cliente envia una lista de instructores
                {
                    if (request.ListaInstructor.Count > 0) //Si tiene mas de 1 instructor
                    {
                        /*Eliminar los instructores actuales del curso en la base de datos*/
                        //Obtengo la lista de instructores de un curso especificado por ID, Devuelve una lista de codigos de instructores en GUID
                        var instructoresBD = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId);
                        foreach (var instructorEliminar in instructoresBD)
                        {
                            //Itero y voy eliminando todos los instructores del curso
                            _context.CursoInstructor.Remove(instructorEliminar);
                        }
                        /*Fin del procedimiento para eliminar instructores*/

                        /*Procedimiento para agregar instructores que provienen del cliente*/
                        foreach (var id in request.ListaInstructor)
                        {
                            var nuevoInstructor = new CursoInstructor
                            {
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                        /*Fin del procedimiento*/
                    }
                }
                //====

                //SaveChangesAsync: Devuelve la cantidad de registros afectados
                //Aca recien se guarda todo en la Base de Datos
                var resultado = await _context.SaveChangesAsync();

                if (resultado > 0) //Si es mayor a 0, se modifico correctamente
                    return Unit.Value; //Retorno un flag

                //Si no se guardo, lanzo una excepcion
                throw new Exception("No se guardaron los cambios en el curso");
            }
        }
    }
}
