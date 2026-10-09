using System.Collections.Generic;

namespace OdontoSoft.Entities.ViewModels
{
    // Tratamiento enriquecido con la lista de precios por ubicación.
    // Es lo que consume el front en la pestaña "Servicios" de Ajustes.
    public class TratamientoConPreciosViewModel
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Disabled { get; set; }
        public List<PrecioPorUbicacion> Precios { get; set; } = new List<PrecioPorUbicacion>();
    }

    public class PrecioPorUbicacion
    {
        public int UbicacionId { get; set; }
        public string UbicacionNombre { get; set; }
        public decimal Precio { get; set; }
    }
}
