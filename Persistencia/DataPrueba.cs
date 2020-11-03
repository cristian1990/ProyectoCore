using Dominio;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia
{
    public class DataPrueba
    {
        //Este metodo inserta datos de prueba en la BD 
        public static async Task InsertarData(CursosOnlineContext context, UserManager<Usuario> usuarioManager)
        {
            //Indico que solamente cree un usuario si no existe ninguno en la BD (Solo se ejecutara 1 vez)
            if (!usuarioManager.Users.Any())
            {
                //Hago la instancia del usuario
                var usuario = new Usuario { NombreCompleto = "Cristian Silva", UserName = "Crismo", Email = "cristian@gmail.com" };
                //Indico el usuario a crear y le paso la Contraseña
                await usuarioManager.CreateAsync(usuario, "Password123$"); 
            }
        }
    }
}
