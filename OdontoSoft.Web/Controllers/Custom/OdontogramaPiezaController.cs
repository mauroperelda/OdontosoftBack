using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OdontoSoft.Web.Controllers
{
  public partial class OdontogramaPiezaController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(OdontogramaPieza entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      SelectList historiaClinicaList = new SelectList(_unitOfWork.HistoriaClinicaRepository.GetAllEnabled(entity.HistoriaClinicaId).ToList(), "Id", "GetString", entity.HistoriaClinicaId);
      viewBagData.Add("historiaClinica", historiaClinicaList);
      return viewBagData;
    }

    private void PrepareEntityFromFormData(OdontogramaPieza entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(OdontogramaPieza entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(OdontogramaPieza entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(OdontogramaPieza entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
  }
}

