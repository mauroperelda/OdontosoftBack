using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class ImagenPacienteViewModel
	{
		public ImagenPacienteViewModel() { }
		public ImagenPacienteViewModel(ImagenPaciente i)
		{
			Id = i.Id;
			HistoriaClinicaId = i.HistoriaClinicaId;
			NombreArchivo = i.NombreArchivo;
			RutaArchivo = i.RutaArchivo;
			MimeType = i.MimeType;
			TamanoBytes = i.TamanoBytes;
			Descripcion = i.Descripcion;
			FechaSubida = i.FechaSubida;
		}

		public int Id { get; set; }
		public int HistoriaClinicaId { get; set; }
		public string NombreArchivo { get; set; }
		public string RutaArchivo { get; set; }
		public string MimeType { get; set; }
		public long TamanoBytes { get; set; }
		public string? Descripcion { get; set; }
		public DateTime FechaSubida { get; set; }

		public void MapViewModelToImagenPaciente(ImagenPaciente i)
		{
			i.HistoriaClinicaId = HistoriaClinicaId;
			i.NombreArchivo = NombreArchivo;
			i.RutaArchivo = RutaArchivo;
			i.MimeType = MimeType;
			i.TamanoBytes = TamanoBytes;
			i.Descripcion = Descripcion;
			i.FechaSubida = FechaSubida;
		}
	}
}
