using System.Collections.Generic;

namespace OdontoSoft.Entities.ViewModels
{
    public class UbicacionViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string? CodigoPostal { get; set; }
        public string Ciudad { get; set; }
        public string? Provincia { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Disabled { get; set; }
    }
}
