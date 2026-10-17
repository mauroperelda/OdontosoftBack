using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class HistoriaClinicaRepository : GenericRepository<HistoriaClinica>, IHistoriaClinicaRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override HistoriaClinica GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    public override IQueryable<HistoriaClinica> AllForBrowser()
    {
      {
        return base.AllForBrowser()
            .Include(x => x.Paciente);
      }
    }
    //public HistoriaClinica GetByMiCampo(int miCampoId)
    //{{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}}
    //public IQueryable<HistoriaClinica> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public HistoriaClinica GetByPacienteId(int pacienteId)
    {
      return All().FirstOrDefault(x => x.PacienteId == pacienteId);
    }

    public HistoriaClinica GetFullByPacienteId(int pacienteId)
    {
      return All()
              .Include(x => x.Paciente)
              .Include(x => x.OdontogramaPiezas)
              .Include(x => x.FichasTratamiento)
                  .ThenInclude(f => f.Tratamiento)
              .Include(x => x.Periodontogramas)
                  .ThenInclude(p => p.Piezas)
              .Include(x => x.ImagenesPaciente)
              .FirstOrDefault(x => x.PacienteId == pacienteId);
    }

    public HistoriaClinica GetHistoriaClinicaById(int id)
    {
      return All()
              .Include(x => x.Paciente)
              .Include(x => x.OdontogramaPiezas)
              .Include(x => x.FichasTratamiento)
                  .ThenInclude(f => f.Tratamiento)
              .FirstOrDefault(x => x.Id == id);
    }
  }
}

