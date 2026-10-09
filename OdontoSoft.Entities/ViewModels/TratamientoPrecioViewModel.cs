namespace OdontoSoft.Entities.ViewModels
{
    // Precio de un Tratamiento en una Ubicación específica.
    public class TratamientoPrecioViewModel
    {
        public int Id { get; set; }
        public int TratamientoId { get; set; }
        public int UbicacionId { get; set; }
        public decimal Precio { get; set; }
    }
}
