using OdontoSoft.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.ViewModels
{
    public class ProfesionalViewModel
    {
        public ProfesionalViewModel()
        {
            
        }

        public ProfesionalViewModel(Profesional profesional)
        {
            Id = profesional.Id;
            Nombre = profesional.Nombre;
            Apellido = profesional.Apellido;
            Dni = profesional.Dni;
            Celular = profesional.Celular;
            FechaNacimiento = profesional.FechaNacimiento;
            DomicilioCalle = profesional.Domicilio;
            ProvinciaId = profesional.ProvinciaId;
            LocalidadId = profesional.LocalidadId;
            Email = profesional.Email;
            Cuit = profesional.Cuit;
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
        public int? DomicilioNumero { get; set; }
        public int? ProvinciaId { get; set; }
        public string? Provincia { get; set; }
        public int? LocalidadId { get; set; }
        public string? Localidad { get; set; }
        public string? Email { get; set; }
        public string? Cuit { get; set;}

        public void MapViewModelToPaciente(Profesional profesional)
        {
            profesional.Nombre = Nombre;
            profesional.Apellido = Apellido;
            profesional.Dni = Dni;
            profesional.Celular = Celular;
            profesional.FechaNacimiento = FechaNacimiento;
            profesional.Domicilio = DomicilioCalle;
            profesional.ProvinciaId = ProvinciaId;
            profesional.LocalidadId = LocalidadId;
            profesional.Email = Email;
            profesional.Cuit = Cuit;
        }
    }
}
