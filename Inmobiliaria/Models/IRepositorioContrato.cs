using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        IList<Contrato> ObtenerTodosDonde(int IdInmueble, string fechaDeInicio, string fechaDeFinalizacion);
    }
}
