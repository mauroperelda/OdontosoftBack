using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  /// <summary>
  /// Imagen asociada a la historia clínica de un paciente
  /// (radiografías, fotos intraorales, documentos escaneados).
  /// El archivo físico vive en wwwroot/uploads/pacientes/{hcId}/{guid}.{ext};
  /// acá solo guardamos la ruta relativa + metadatos.
  /// </summary>
  public class ImagenPaciente : AuditedEntity
  {
    public int HistoriaClinicaId { get; set; }
    public HistoriaClinica HistoriaClinica { get; set; }
    /// <summary>Nombre original que tenía el archivo en el cliente.</summary>
    public string NombreArchivo { get; set; }
    /// <summary>Ruta relativa servida por el controller, ej: "uploads/pacientes/1/abc.jpg".</summary>
    public string RutaArchivo { get; set; }
    public string MimeType { get; set; }
    public long TamanoBytes { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaSubida { get; set; }
  }
}
