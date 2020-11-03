using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Comentarios
{
    public class Eliminar
    {
        //Clase para data que envia el cliente
        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        //Clase para la logica de eliminar un comentario
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Busco el comentario mediante su ID
                var comentario = await _context.Comentario.FindAsync(request.Id);
                
                if (comentario == null) //Si no lo encuentro
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el comentario" });
                }

                _context.Remove(comentario); //Indico que quiero eliminar

                var resultado = await _context.SaveChangesAsync(); //Elimino de la BD
                
                if (resultado > 0) //Verifico si el nro. de registros afectados es mayor que 0
                {
                    return Unit.Value; //Retorno un flag (success)
                }

                throw new Exception("No se pudo eliminar el comentario");
            }
        }
    }
}
