using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{

    public class Inmueble
    {
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        [Display(Name = "Cantidad de Ambientes")]
        public int CantDeAmbientes { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        [Display(Name = "Tipo de Uso")]
        public string TipoDeUso { get; set; }   
        [Required]
        [Display(Name = "Tipo de Inmueble")]
        public string TipoDeInmueble { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public int Precio { get; set; }
        [Display(Name = "Codigo del Propietario")]
        public int IdPropietario { get; set; }
        [ForeignKey("IdPropietario")]
        public Propietario Propietario { get; set; }

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return roles;
        }

        public static IList<String> ObtenerTiposDeUsos()
        {          
            return new List<String> { "Comercial", "Residencial" };
        }

        public static IList<String> ObtenerEstados()
        {
            return new List<String> { "Disponible", "Suspendido" };
        }

        public static IList<String> ObtenerTiposDeInmuebles()
        {
            return new List<String> { "Local", "Deposito", "Casa", "Departamento" };
        }
    }
}
