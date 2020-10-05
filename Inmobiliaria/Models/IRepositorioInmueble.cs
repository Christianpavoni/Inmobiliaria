using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> ObtenerTodosDonde(int IdPropietario, string Estado);
        IList<Inmueble> ObtenerTodosDisponiblesPorFechas(string fi,string ff);
    }
}
