using Microsoft.AspNetCore.Mvc.ModelBinding;
using OdontoSoft.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Contracts.Dtos
{
  public class TurnoDto
  {
    public TurnoDto()
    {

    }
    public TurnoDto(Turno turno)
    {
      Id = turno.Id;
      PacienteId = turno.PacienteId;
      PacienteObj = turno.Paciente;
      Paciente = turno.Paciente?.Apellido + ", " + turno.Paciente?.Nombre;
      ProfesionalId = turno.ProfesionalId;
      ProfesionalObj = turno.Profesional;
      Profesional = turno.Profesional?.Apellido + ", " + turno.Profesional?.Nombre;
      Fecha = turno.Fecha;
      HoraInicio = turno.HoraInicio;
      HoraFin = turno.HoraFin;
      Observacion = turno.Observacion;
      TratamientoId = turno.TratamientoId;
      TratamientoObj = turno.Tratamiento;
      EstadoTurnoId = turno.EstadoTurnoId;
      EstadoTurnoObj = turno.EstadoTurno;
      ColorEstado = turno.EstadoTurno?.ColorHex;
      Dni = turno.Paciente?.Dni;
      UbicacionId = turno.UbicacionId;
      UbicacionObj = turno.Ubicacion;
      Ubicacion = turno.Ubicacion?.Nombre;
    }
    //[BindNever]
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string? Paciente { get; set; }
    // Objeto FK completo del paciente. Nullable porque en teoría un turno
    // sin paciente cargado devolvería null.
    public Paciente? PacienteObj { get; set; }
    public int ProfesionalId { get; set; }
    public string? Profesional { get; set; }
    public Profesional? ProfesionalObj { get; set; }
    public DateTime? Fecha { get; set; }
    [DisplayName("Hora Inicio")]
    public DateTime? HoraInicio { get; set; }
    [DisplayName("Hora Fin")]
    public DateTime? HoraFin { get; set; }
    public string? Observacion { get; set; }
    public List<TratamientoDto>? Tratamientos { get; set; }
    public int? TratamientoId { get; set; }
    public Tratamiento? TratamientoObj { get; set; }
    public string? Dni { get; set; }
    public string? ColorEstado { get; set; }
    public int? EstadoTurnoId { get; set; }
    public EstadoTurno? EstadoTurnoObj { get; set; }
    public int? UbicacionId { get; set; }
    public string? Ubicacion { get; set; }
    public Ubicacion? UbicacionObj { get; set; }

    public void MapDtoToTurno(Turno turno)
    {
      turno.PacienteId = PacienteId;
      turno.ProfesionalId = ProfesionalId;
      turno.Fecha = (DateTime)Fecha;
      turno.HoraInicio = (DateTime)HoraInicio;
      turno.HoraFin = HoraFin;
      turno.Observacion = Observacion;
      turno.TratamientoId = TratamientoId;
      turno.EstadoTurnoId = EstadoTurnoId;
      // Normalizamos 0 → null para no guardar un FK inválido.
      turno.UbicacionId = (UbicacionId == 0) ? null : UbicacionId;
    }
  }
}