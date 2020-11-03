using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Instructores
{
    public class Editar
    {
        //Clase que contiene las propiedades enviadas por el cliente
        public class Ejecuta : IRequest
        {
            public Guid InstructorId { get; set; }
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Titulo { get; set; }

            //El UTC de FechaCreacion lo asigno desde el StoredProcedure
        }

        //Clase para validar la data enviada
        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.Nombre).NotEmpty(); //Regla para, No vacio
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Titulo).NotEmpty();
            }
        }

        //Clase para la logica de actualizacion en la BD
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepositorio;

            public Manejador(IInstructor instructorRepositorio)
            {
                _instructorRepositorio = instructorRepositorio;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Creo una instancia de Actualiza y paso como parametro lo que llega del cliente
                var resultado = await _instructorRepositorio.Actualiza(request.InstructorId, request.Nombre, request.Apellidos, request.Titulo);
                
                if (resultado > 0) //Verifico si se afecto mas de 1 registro
                {
                    return Unit.Value; //Retorno un Flag (success)
                }

                throw new Exception("No se pudo actualizar la data del instructor");
            }
        }
    }
}
