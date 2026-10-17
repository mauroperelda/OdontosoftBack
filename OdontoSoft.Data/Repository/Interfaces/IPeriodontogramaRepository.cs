using OdontoSoft.Domain.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IPeriodontogramaRepository : IGenericRepository<Periodontograma>
  {
    /// <summary>Devuelve el último periodontograma (por FechaRegistro) de una HC, con piezas cargadas.</summary>
    Periodontograma GetLastByHistoriaClinicaId(int historiaClinicaId);

    /// <summary>Devuelve todos los periodontogramas de una HC ordenados desc por fecha.</summary>
    IQueryable<Periodontograma> GetByHistoriaClinicaId(int historiaClinicaId);

    /// <summary>Devuelve un periodontograma con sus piezas cargadas explícitamente.</summary>
    Periodontograma GetWithPiezasById(int id);

    /// <summary>Agrega una pieza al DbSet (no guarda cambios). Llamar Save() después.</summary>
    void AddPieza(PeriodontogramaPieza pieza);

    /// <summary>Borra todas las piezas de un periodontograma (no guarda cambios). Llamar Save() después.</summary>
    void DeletePiezas(int periodontogramaId);
  }

  /// <summary>
  /// Métricas agregadas de una evaluación periodontal, calculadas a partir
  /// de las piezas (no se persisten — se calculan en cada GET).
  /// </summary>
  public class PeriodontogramaMetricas
  {
    public decimal BopPorcentaje { get; set; }
    public decimal PlacaPorcentaje { get; set; }
    public int DientesPresentes { get; set; }
    public int DientesAusentes { get; set; }
  }
}
