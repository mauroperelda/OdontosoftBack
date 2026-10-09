using OdontoSoft.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.ViewModels
{
    public class PacienteViewModel
    {
        public PacienteViewModel()
        {
            
        }
        public PacienteViewModel(Paciente paciente)
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

        public void MapViewModelToPaciente(Paciente paciente)
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
