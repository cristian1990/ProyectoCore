using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Seguridad
{
    //Esta es una clase DTO, que sirve para enbviar al cliente los datos que yo quiero
    public class UsuarioData
    {
        public string NombreCompleto { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Imagen { get; set; }
    }
}
