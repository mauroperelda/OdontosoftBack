using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class PeriodontogramaRepository : GenericRepository<Periodontograma>, IPeriodontogramaRepository
  {
    public PeriodontogramaRepository(
      OdontosoftContext context,
      ILogger logger,
      IAuthenticatedUser authenticatedUser)
      : base(context, logger, authenticatedUser)
    {
    }

    public Periodontograma GetLastByHistoriaClinicaId(int historiaClinicaId)
    {
      return All()
        .Include(x => x.Piezas)
        .Where(x => x.HistoriaClinicaId == historiaClinicaId)
        .OrderByDescending(x => x.FechaRegistro)
        .FirstOrDefault();
    }

    public IQueryable<Periodontograma> GetByHistoriaClinicaId(int historiaClinicaId)
    {
      return All()
        .Include(x => x.Piezas)
        .Where(x => x.HistoriaClinicaId == historiaClinicaId)
        .OrderByDescending(x => x.FechaRegistro);
    }

    public Periodontograma GetWithPiezasById(int id)
    {
      return All()
        .Include(x => x.Piezas)
        .FirstOrDefault(x => x.Id == id);
    }

    public void AddPieza(PeriodontogramaPieza pieza)
    {
      _context.PeriodontogramaPieza.Add(pieza);
    }

    public void DeletePiezas(int periodontogramaId)
    {
      var piezas = _context.PeriodontogramaPieza
        .Where(p => p.PeriodontogramaId == periodontogramaId)
        .ToList();
      if (piezas.Count > 0)
        _context.PeriodontogramaPieza.RemoveRange(piezas);
    }
  }
}
