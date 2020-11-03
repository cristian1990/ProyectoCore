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
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            //Data que enviara el cliente
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Titulo { get; set; }
            
            //El UTC de FechaCreacion lo asigno desde el StoredProcedure
        }

        //Clase para validar las propiedades
        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.Nombre).NotEmpty(); //Regla para, No vacio
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Titulo).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepository;
            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Creo una instancia de nuevo y paso como parametro lo que llega del cliente
                var resultado = await _instructorRepository.Nuevo(request.Nombre, request.Apellidos, request.Titulo);

                if (resultado > 0) //Verifico si se afecto mas de 1 registro
                {
                    return Unit.Value; //Retorno un Flag (success)
                }

                throw new Exception("No se pudo insertar el instructor");
            }
        }
    }
}
