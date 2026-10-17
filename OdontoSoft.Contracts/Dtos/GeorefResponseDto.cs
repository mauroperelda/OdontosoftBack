using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdontoSoft.Entities.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class GeorefResponseDto
  {
    public int cantidad { get; set; }
    public int total { get; set; }
    public List<LocalidadGeoref>? localidades { get; set; }
  }

  public class LocalidadGeoref
  {
    public string? nombre { get; set; }
    public ProvinciaGeoref? provincia { get; set; }
  }

  public class ProvinciaGeoref
  {
    public string? id { get; set; }
    public string? nombre { get; set; }
  }
}