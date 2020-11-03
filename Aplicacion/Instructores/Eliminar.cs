using MediatR;
using Persistencia.DapperConexion.Instructor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Instructores
{
    public class Eliminar
    {
        //Clase de que contiene la propiedad ID que envia el cliente
        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        //Logica para eliminar el Instructor, mediante su ID
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepositorio;
            public Manejador(IInstructor instructorRepositorio)
            {
                _instructorRepositorio = instructorRepositorio;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco el Instructor en la BD mediante su ID
                var resultados = await _instructorRepositorio.Elimina(request.Id);
                
                if (resultados > 0) //Verifico si se afecto mas de 1 registro
                {
                    return Unit.Value; //Retorno un Flag (success)
                }

                throw new Exception("No se pudo eliminar el instructor");
            }
        }
    }
}
