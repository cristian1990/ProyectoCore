
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    //IdentityUser: contiene las propiedades de Identity con las que trabaja un usuario
    //Por ejemplo tiene Email, Password, PhoneNumber... Heredo todo en la clase
    public class Usuario : IdentityUser
    {
        //Creo una nueva propiedad adicional a las que heredo de IdentityUser
        public string NombreCompleto { get; set; }
    }
}
