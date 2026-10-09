using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class EstadoTurnoViewModel
	{
		public EstadoTurnoViewModel() { }

		public EstadoTurnoViewModel(EstadoTurno e)
		{
			Id = e.Id;
			Nombre = e.Nombre;
			ColorHex = e.ColorHex;
		}

		public int Id { get; set; }
		public string Nombre { get; set; }
		public string ColorHex { get; set; }

		public void MapViewModelToEstadoTurno(EstadoTurno e)
		{
			e.Nombre = Nombre;
			e.ColorHex = ColorHex;
		}
	}
}