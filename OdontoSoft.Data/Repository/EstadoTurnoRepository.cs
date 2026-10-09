using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
    public partial class EstadoTurnoRepository : GenericRepository<EstadoTurno>, IEstadoTurnoRepository
    {
        /*Ejemplo de reemplazo de métodos existentes*/
        //public override EstadoTurno GetById(int id)
        //{{
        //    return All()
        //        .Include(x => x.ProvinciaId)
        //        .FirstOrDefault(x=>x.Id == id);
        //}}
        //public override IQueryable<EstadoTurno> AllForBrowser()
        //{{
        //    return base.AllForBrowser()
        //        .Include(x => x.Provincia)
        //        .OrderBy(x=>x.Descripcion);
        //}}
        //public EstadoTurno GetByMiCampo(int miCampoId)
        //{{
        //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
        //}}
        //public IQueryable<EstadoTurno> GetByMiOtroCampo(int miOtroCampoId)
        //{{
        //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
        //}}
    }
}

