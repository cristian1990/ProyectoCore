using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : MiControllerBase
    {
        //Hacemos la inyeccion de dependencias para el DbContext
        private readonly CursosOnlineContext context;
        public WeatherForecastController(CursosOnlineContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public IEnumerable<Curso> Get()
        {
            return context.Curso.ToList();
        }
    }
}
