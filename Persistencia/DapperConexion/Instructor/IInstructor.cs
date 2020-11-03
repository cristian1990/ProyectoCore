using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
        Task<IEnumerable<InstructorModel>> ObtenerLista();

        Task<InstructorModel> ObtenerPorId(Guid id);

        //Pongo que devuelve un int, porque al hacer SaveChanges es lo que me devuelve, los registros afectados
        Task<int> Nuevo(string nombre, string apellidos, string titulo);

        Task<int> Actualiza(Guid instructorId, string nombre, string apellidos, string titulo);

        Task<int> Elimina(Guid id);
    }
}
