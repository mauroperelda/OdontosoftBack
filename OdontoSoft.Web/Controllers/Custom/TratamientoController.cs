using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Web.Controllers
{
  public partial class TratamientoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Tratamiento entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Tratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Tratamiento entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Tratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Tratamiento entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }

    [HttpGet]
    public JsonResult GetTratamientos()
    {
      try
      {
        var tratamientos = _unitOfWork.TratamientoRepository.GetAllEnabled().ToList();
        return JsonOk("tratamientos", tratamientos);
      }
      catch (Exception ex)
      {
        return JsonError("Error al cargar los tratamientos " + ex.Message);
      }

    }
  }
}
