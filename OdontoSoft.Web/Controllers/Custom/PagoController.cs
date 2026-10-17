using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OdontoSoft.Web.Controllers
{
  public partial class PagoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Pago entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      SelectList presupuestoList = new SelectList(_unitOfWork.PresupuestoRepository.GetAllEnabled(entity.PresupuestoId).ToList(), "Id", "GetString", entity.PresupuestoId);
      viewBagData.Add("presupuesto", presupuestoList);
      return viewBagData;
    }

    private void PrepareEntityFromFormData(Pago entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(Pago entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(Pago entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Pago entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
  }
}

