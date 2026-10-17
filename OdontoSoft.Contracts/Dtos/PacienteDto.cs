using OdontoSoft.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Contracts.Dtos
{
  public class PacienteDto
  {
    public PacienteDto()
    {

    }
    public PacienteDto(Paciente paciente)
    {
      Id = paciente.Id;
      Nombre = paciente.Nombre;
      Apellido = paciente.Apellido;
      Dni = paciente.Dni;
      Telefono = paciente.Telefono;
      Celular = paciente.Celular;
      FechaNacimiento = paciente.FechaNacimiento;
      DomicilioCalle = paciente.DomicilioCalle;
      DomicilioNumero = paciente.DomicilioNumero;
      //Provincia = paciente.Provincia;
      ProvinciaId = paciente.ProvinciaId;
      //Localidad = paciente.Localidad;
      LocalidadId = paciente.LocalidadId;
      Email = paciente.Email;
    }
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Dni { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    [Display(Name = "Fecha de Nacimiento")]
    public DateTime? FechaNacimiento { get; set; }
    [Display(Name = "Domicilio")]
    public string? DomicilioCalle { get; set; }
    [Display(Name = "Numero")]
    public int? DomicilioNumero { get; set; }
    public int? ProvinciaId { get; set; }
    public string? Provincia { get; set; }
    public int? LocalidadId { get; set; }
    public string? Localidad { get; set; }
    public string? Email { get; set; }
    //public string NombreCompleto { get; set; }

    // Kept indefinitely until OdontoSoft.Web is deprecated or migrated
    // to OdontoSoft.API. En la API ya se reemplaza por AutoMapper; este
    // mapping manual sigue siendo necesario para que el MVC legacy
    // siga funcionando mientras conviven ambos frontends.
    public void MapDtoToPaciente(Paciente paciente)
    {
      paciente.Nombre = Nombre;
      paciente.Apellido = Apellido;
      paciente.Dni = Dni;
      paciente.Celular = Celular;
      paciente.FechaNacimiento = FechaNacimiento;
      paciente.DomicilioCalle = DomicilioCalle;
      paciente.ProvinciaId = ProvinciaId;
      paciente.LocalidadId = LocalidadId;
      paciente.Email = Email;
    }
  }
}