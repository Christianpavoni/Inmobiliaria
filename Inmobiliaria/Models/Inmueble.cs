using System;
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
    }
}
