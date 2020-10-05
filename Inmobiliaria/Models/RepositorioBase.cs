using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;
        

        public RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public static string mensajeExitoso(string action)
        {
            if (action == "create" || action == "edit")
            {
                return "Datos guardados correctamente";
            }
            if(action == "delete")
            {
                return "Eliminación realizada correctamente";
            }
            
            return "";
            
        }

        public static string mensajeError(string action)
        {
            if (action == "create")
            {
                return "Error en la Creacion";
            }
            if (action == "edit")
            {
                return "Error en la Edicion";
            }

            if (action == "delete")
            {
                return "Error en la Eliminacion";
            }

            if (action == "fechas")
            {
                return "Error en la eleccion de las fechas";
            }

            return "";

        }

        public static string mensajeErrorInsert(string entidad)
        {
            return "Inserte algun" + entidad + " primero";

        }

    }
}
