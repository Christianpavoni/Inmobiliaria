using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "Código")]
        public int IdPago { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public int Importe { get; set; }
        [Required]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.Date)]
        public DateTime FechaDePago { get; set; }
        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }
        [ForeignKey("IdContrato")]
        public Contrato Contrato { get; set; }
    }
}
