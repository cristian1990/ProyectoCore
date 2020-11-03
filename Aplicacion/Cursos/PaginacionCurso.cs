using MediatR;
using Persistencia.DapperConexion.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    public class PaginacionCurso
    {
        //Clase que contiene las propiedades que envia el cliente para filtrar
        public class Ejecuta : IRequest<PaginacionModel> //Tipo de dato quye voy a retornar
        {
            public string Titulo { get; set; }
            public int NumeroPagina { get; set; }
            public int CantidadElementos { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, PaginacionModel>
        {
            private readonly IPaginacion _paginacion;
            public Manejador(IPaginacion paginacion)
            {
                _paginacion = paginacion;
            }

            public async Task<PaginacionModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var storedProcedure = "usp_obtener_curso_paginacion"; //Nombre del SP que quiero ejecutar
                var ordenamiento = "Titulo"; //Indico que ordene por Titulo
                var parametros = new Dictionary<string, object>(); //Parametros de filtro
                parametros.Add("NombreCurso", request.Titulo); //Indico el nombre del parametro y su valor (Titulo)
                
                //Retorno la paginacion con todos los paramentros necesarios
                return await _paginacion.devolverPaginacion(storedProcedure, request.NumeroPagina, request.CantidadElementos, parametros, ordenamiento);
            }
        }
    }
}
