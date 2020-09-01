using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
		[Key]
		[Display(Name = "Código")]
		public int IdInquilino { get; set; }
		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }
		[Required]
		public string Dni { get; set; }
		[Required]
		public string Telefono { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
		[Required]
		[Display(Name = "Lugar de Trabajo")]
		public string LugarDeTrabajo { get; set; }
		[Required]
		[Display(Name = "Nombre Garante")]
		public string NombreGarante { get; set; }
		[Required]
		[Display(Name = "Dni Garante")]
		public string DniGarante { get; set; }
		[Required]
		[Display(Name = "Tel. Garante")]
		public string TelefonoGarante { get; set; }
		[Required, EmailAddress]
		[Display(Name = "Email Garante")]
		public string EmailGarante { get; set; }
	}
}
