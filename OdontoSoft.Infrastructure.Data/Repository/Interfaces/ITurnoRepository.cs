using OdontoSoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Infrastructure.Data.Repository.Interfaces
{
  public interface ITurnoRepository : IGenericRepository<Turno>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en LocalidadRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //Localidad GetByMiCampo(int miCampoId);

    //Ejemplo custom de getBy con una lista de resultado
    //IQueryable<Localidad> GetByMiOtroCampo(int miOtroCampoId);

    IQueryable<Turno> GetAll();
    Turno GetByProfesionalByHoraInicio(int profesionalId, DateTime? horaInicio);
    Turno GetByTurnoId(int id);
  }
}
