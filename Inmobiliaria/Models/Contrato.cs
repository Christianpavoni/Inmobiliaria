using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        [Display(Name = "Código")]
        public int IdContrato { get; set; }
        [Required]
        public string Detalle { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto de Alquiler Mensual")]
        public int Monto { get; set; }
        [Required]
        [Display(Name = "Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaDeInicio { get; set; }
        [Required]
        [Display(Name = "Finalizacion")]
        [DataType(DataType.Date)]
        public DateTime FechaDeFinalizacion { get; set; }
        [Required]
        [Display(Name = "Codigo del Inquilino")]
        public int IdInquilino { get; set; }
        [ForeignKey("IdInquilino")]
        public Inquilino Inquilino { get; set; }

        [Display(Name = "Codigo del Inmueble")]
        public int IdInmueble { get; set; }
        [ForeignKey("IdInmueble")]
        public Inmueble Inmueble { get; set; }



    }
}

