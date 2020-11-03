using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Comentarios
{
    public class Nuevo
    {
        //Clase que contiene campos que envia el cliente
        public class Ejecuta : IRequest
        {
            public string Alumno { get; set; }
            public int Puntaje { get; set; }
            public string Comentario { get; set; }
            public Guid CursoId { get; set; }
        }

        //Clase para validar campos
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Alumno).NotEmpty(); //Regla para, No vacio
                RuleFor(x => x.Puntaje).NotEmpty();
                RuleFor(x => x.Comentario).NotEmpty();
                RuleFor(x => x.CursoId).NotEmpty();
            }
        }

        //Clase para la logica de agregar un comentario
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var comentario = new Comentario //Creo un objeto comentario
                {
                    ComentarioId = Guid.NewGuid(), //Id aleatorio de tipo Guid
                    Alumno = request.Alumno,
                    ComentarioTexto = request.Comentario,
                    CursoId = request.CursoId,
                    FechaCreacion = DateTime.UtcNow //Asigno el tiempo actual con Utc para evitar problema con las ubicaciones los clientes
                };

                _context.Comentario.Add(comentario); //Indico que quiero agregar

                var resultados = await _context.SaveChangesAsync(); //Agrego a la BD
               
                if (resultados > 0) //Si nro. de registros afectados es mayor a 0
                {
                    return Unit.Value; //Envio un flag (Success)
                }

                throw new Exception("No se pudo insertar el comentario");
            }
        }
    }
}
