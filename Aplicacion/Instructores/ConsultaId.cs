using Aplicacion.ManejadorError;
using MediatR;
using Persistencia.DapperConexion.Instructor;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Instructores
{
    public class ConsultaId
    {
        //Clase de propiedad que envia el cliente
        public class Ejecuta : IRequest<InstructorModel> //Indico que retorno un InstructorModel
        {
            public Guid Id { get; set; }
        }

        //Clase para la logica para buscar el Instructor por ID
        public class Manejador : IRequestHandler<Ejecuta, InstructorModel> //Indico que retorno un InstructorModel
        {
            private readonly IInstructor _instructorRepository;
            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<InstructorModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco el Instructor en la BD mediante su ID
                var instructor = await _instructorRepository.ObtenerPorId(request.Id);
                
                if (instructor == null) //Si no existe el instructor
                {
                    //Envio un 404
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el instructor" });
                }

                return instructor; //Si existe, lo retorno
            }
        }
    }
}
