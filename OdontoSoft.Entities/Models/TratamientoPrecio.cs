using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
    // Tabla intermedia que ata un Tratamiento (servicio) con una Ubicación
    // y le asigna un Precio. Permite que el mismo servicio cueste distinto
    // según la sede.
    public class TratamientoPrecio : AuditedEntity
    {
        public int TratamientoId { get; set; }
        public Tratamiento Tratamiento { get; set; }

        public int UbicacionId { get; set; }
        public Ubicacion Ubicacion { get; set; }

        public decimal Precio { get; set; }
    }
}
