using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OdontoSoft.Web.Controllers
{
  public partial class PresupuestoDetalleController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(PresupuestoDetalle entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      SelectList presupuestoList = new SelectList(_unitOfWork.PresupuestoRepository.GetAllEnabled(entity.PresupuestoId).ToList(), "Id", "GetString", entity.PresupuestoId);
      viewBagData.Add("presupuesto", presupuestoList);
      SelectList tratamientoList = new SelectList(_unitOfWork.TratamientoRepository.GetAllEnabled(entity.TratamientoId).ToList(), "Id", "GetString", entity.TratamientoId);
      viewBagData.Add("tratamiento", tratamientoList);
      return viewBagData;
    }

    private void PrepareEntityFromFormData(PresupuestoDetalle entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(PresupuestoDetalle entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(PresupuestoDetalle entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(PresupuestoDetalle entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
  }
}

