using Mauro.Entities.Base;
using OdontoSoft.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.Models
{
	public class Turno : AuditedEntity
	{
		public int Id { get; set; }
		public int PacienteId { get; set; }
		public Paciente Paciente { get; set; }
		public int ProfesionalId { get; set; }
		public Profesional Profesional { get; set; }
		public DateTime Fecha { get; set; }
		public DateTime HoraInicio { get; set; }
		public DateTime? HoraFin { get; set; }
		public string? Observacion { get; set; }
		public Tratamiento Tratamiento { get; set; }
		public int? TratamientoId { get; set; }
		public int? EstadoTurnoId { get; set; }
		public EstadoTurno? EstadoTurno { get; set; }
		// Opcional: el consultorio / sucursal donde se atiende el turno.
		// Nullable para no romper turnos existentes que no tenían este FK.
		public int? UbicacionId { get; set; }
		public Ubicacion? Ubicacion { get; set; }

	}
}
