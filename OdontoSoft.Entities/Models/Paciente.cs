using OdontoSoft.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;


namespace OdontoSoft.Entities.Models
{
	public class Paciente : AuditedEntity
	{
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public string Dni { get; set; }
		public string? Telefono { get; set; }
		public string? Celular { get; set; }
		[Display(Name = "Fecha de Nacimiento")]
		public DateTime? FechaNacimiento { get; set; }
		[Display(Name = "Domicilio")]
		public string? DomicilioCalle { get; set; }
		[Display(Name = "Numero")]
		public int? DomicilioNumero { get; set; }
		public int? ProvinciaId { get; set; }
		public Provincia? Provincia { get; set; }
		public int? LocalidadId { get; set; }
		public Localidad? Localidad { get; set; }
		public string? Email { get; set; }

		public override string ToString()
		{
			return $"{Apellido}, {Nombre}";
		}
	}
}
