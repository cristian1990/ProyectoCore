using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    //Clase que devuelve todos los Roles de la BD
    public class RolLista
    {
        //Clase que devolvera una lista de Roles, sin filtrar por nada, por eso no hay parametros de entrada
        public class Ejecuta : IRequest<List<IdentityRole>>
        {
        }

        public class Manejador : IRequestHandler<Ejecuta, List<IdentityRole>>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<List<IdentityRole>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Obtengo una lista de todos los Roles de la tabla AspNetRoles
                var roles = await _context.Roles.ToListAsync();
                return roles;
            }
        }
    }
}
