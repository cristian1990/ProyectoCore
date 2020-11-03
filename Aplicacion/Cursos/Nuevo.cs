using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            //Validacion con Data Annotations
            //[Required(ErrorMessage = "Por favor ingrese el titulo del curso")]
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; } // ?: Indico permite null, para evitar errores de validacion
            public List<Guid> ListaInstructor { get; set; } //Lista de IDs de instructores en formado GUID
            public decimal Precio { get; set; }
            public decimal Promocion { get; set; }
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

        //Clase que representa la logica de la transaccion
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            //Logica para insertar un nuevo curso
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //==== agregar logica para insertar un curso
                Guid _cursoId = Guid.NewGuid(); //Creo un Guid aleatorio para que sea el ID del curso
                var curso = new Curso //Creo un objeto curso
                {
                    CursoId = _cursoId, //Asigno el ID aleatorio
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow //Asigno el tiempo actual con Utc para evitar problema con las ubicaciones los clientes
                };

                _context.Curso.Add(curso); //Indico que quiero agregar a la BD
                //====

                //==== agregar logica para insertar un instructores al curso
                //Si la lista de IDs existe, es diferente de null (tiene algun instructor agregado)
                if (request.ListaInstructor != null)
                {
                    //Recorro la lista de instructores 
                    foreach (var id in request.ListaInstructor)
                    {
                        //Por cada ID de instructor en la lista creo una nueva instancia de CursoInstructor
                        var cursoInstructor = new CursoInstructor
                        {
                            //Lleno la data de la entidad
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        //Indico que se agregue en la Tabla CursoInstructor de la BD
                        _context.CursoInstructor.Add(cursoInstructor);
                    }
                }
                //====

                //==== agregar logica para insertar un precio del curso
                var precioEntidad = new Precio
                {
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid() //Se creo un Guid aleatorio
                };

                _context.Precio.Add(precioEntidad);
                //====

                //Guardo en la base los cambios en las tablas Curso, CursoInstructor y precio
                //valor contiene el numero de registros afectados
                var valor = await _context.SaveChangesAsync(); 

                if (valor > 0)
                {
                    return Unit.Value; //Si es exitoso retorno un Flag (bandera)
                }

                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}
