using Microsoft.EntityFrameworkCore;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Data.Repository
{
    public partial class ProfesionalRepository : GenericRepository<Profesional>, IProfesionalRepository
    {
        /*Ejemplo de reemplazo de métodos existentes*/

        //public override Provincia GetById(int id)
        //{
        //    return All()
        //        .Include(x => x.ProvinciaId)
        //        .FirstOrDefault(x=>x.Id == id);
        //}

        //Ejemplo para modificar la lista que llena el browser
        public override IQueryable<Profesional> AllForBrowser()
        {
            return base.AllForBrowser()
                .Include(x => x.Provincia)
                .Include(x => x.Localidad)
                .OrderBy(x => x.Apellido);
        }


        /*Ejemplo de nuevos métodos - Recuerde ademas modificar la interfaz IProvinciaRepository*/

        //Ejemplo custom de getBy con un solo resultado
        //public Provincia GetByMiCampo(int miCampoId)
        //{
        //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
        //}

        //Ejemplo custom de getBy con una lista de resultado
        //public IQueryable<Provincia> GetByMiOtroCampo(int miOtroCampoId)
        //{
        //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
        //}
    }
}
