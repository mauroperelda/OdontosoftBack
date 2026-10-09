using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.ViewModels
{
    public class TratamientoViewModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion {  get; set; }
        public bool Disabled { get; set; }
    }
}
