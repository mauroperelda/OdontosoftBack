using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class HistoriaClinicaDto
  {
    public HistoriaClinicaDto()
    {
      Piezas = new List<OdontogramaPiezaDto>();
      Fichas = new List<FichaTratamientoDto>();
    }
    public HistoriaClinicaDto(HistoriaClinica h)
    {
      Id = h.Id;
      PacienteId = h.PacienteId;
      Paciente = h.Paciente?.Nombre + " " + h.Paciente?.Apellido;
      Alergias = h.Alergias;
      Medicamentos = h.Medicamentos;
      AntecedentesMedicos = h.AntecedentesMedicos;
      Observaciones = h.Observaciones;
      Piezas = h.OdontogramaPiezas?
        .Select(p => new OdontogramaPiezaDto(p))
        .ToList() ?? new List<OdontogramaPiezaDto>();

      Fichas = h.FichasTratamiento?
        .Select(f => new FichaTratamientoDto(f))
        .ToList() ?? new List<FichaTratamientoDto>();
    }
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string? Paciente { get; set; }
    public string? Alergias { get; set; }
    public string? Medicamentos { get; set; }
    public string? AntecedentesMedicos { get; set; }
    public string? Observaciones { get; set; }
    public List<OdontogramaPiezaDto>? Piezas { get; set; }
    public List<FichaTratamientoDto>? Fichas { get; set; }

    public void MapDtoToHistoriaClinica(HistoriaClinica h)
    {
      h.PacienteId = PacienteId;
      h.Alergias = Alergias;
      h.Medicamentos = Medicamentos;
      h.AntecedentesMedicos = AntecedentesMedicos;
      h.Observaciones = Observaciones;
    }
  }
}