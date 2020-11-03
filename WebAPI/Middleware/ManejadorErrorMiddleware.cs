using Aplicacion.ManejadorError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Middleware
{
    //Esta clase sirve para interceptar un requerimiento, por ejemplo al dar de alta un curso
    //Verifica si la data enviada es correcta, si tiene el titulo, la descripcion, ect
    public class ManejadorErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorErrorMiddleware> _logger;
        public ManejadorErrorMiddleware(RequestDelegate next, ILogger<ManejadorErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //Si la data es correcta, pasa al proximo procedimiento, la transaccion (insercion)
                await _next(context); 
            }
            catch (Exception ex)
            {
                //Si falta el titulo o no cumple alguna regla, se crea una excepcion y dispara el siguiente metodo
                await ManejadorExcepcionAsincrono(context, ex, _logger);
            }
        }

        //Este metodo indica el tipo de excepcion que se va a lanzar si es de tipo HTTP o Generico de C#
        private async Task ManejadorExcepcionAsincrono(HttpContext context, Exception ex, ILogger<ManejadorErrorMiddleware> logger)
        {
            object errores = null;

            switch (ex) //Evaluamos el tipo de Excepcion
            {
                //HTTP
                case ManejadorExcepcion me:
                    logger.LogError(ex, "Manejador Error"); //Imprime copn detalle el log del error
                    errores = me.Errores; //Almaceno los errores
                    context.Response.StatusCode = (int)me.Codigo; //Asigno el codigo de error HTTP
                    break;
                //Generico de C#
                case Exception e:
                    logger.LogError(ex, "Error de Servidor");
                    errores = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message; //Si es null almacena error, si no almacena como viene 
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //Asigno tipo de error de servidor
                    break;
            }

            context.Response.ContentType = "application/json"; //La respuesta es JSON
            if (errores != null) //Si el objeto tiene algun error
            {
                var resultados = JsonConvert.SerializeObject(new { errores }); //Serializo a JSON
                await context.Response.WriteAsync(resultados); //Envio la respuesta
            } 
        }
    }
}
