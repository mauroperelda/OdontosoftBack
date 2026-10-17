using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OdontoSoft.Web.Controllers
{
  public partial class FichaTratamientoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(FichaTratamiento entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      SelectList historiaClinicaList = new SelectList(_unitOfWork.HistoriaClinicaRepository.GetAllEnabled(entity.HistoriaClinicaId).ToList(), "Id", "GetString", entity.HistoriaClinicaId);
      viewBagData.Add("historiaClinica", historiaClinicaList);
      SelectList turnoList = new SelectList(_unitOfWork.TurnoRepository.GetAllEnabled(entity.TurnoId.HasValue ? entity.TurnoId.Value : 0).ToList(), "Id", "GetString", entity.TurnoId);
      viewBagData.Add("turno", turnoList);
      SelectList tratamientoList = new SelectList(_unitOfWork.TratamientoRepository.GetAllEnabled(entity.TratamientoId).ToList(), "Id", "GetString", entity.TratamientoId);
      viewBagData.Add("tratamiento", tratamientoList);
      return viewBagData;
    }

    private void PrepareEntityFromFormData(FichaTratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(FichaTratamiento entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(FichaTratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(FichaTratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
  }
}

