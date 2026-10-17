using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using OdontoSoft.Web.Browser;

namespace OdontoSoft.Web.Controllers
{
  public partial class ProvinciaController : AppBaseBrowserController
  {

    private Dictionary<string, object> PrepareViewBag(Provincia entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Provincia entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Provincia entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Provincia entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Provincia entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }


    //public JsonResult Sincronizar()
    //{
    //    try
    //    {                
    //        var ws = new ExtraCore.Superintendencia.Webservices.SuperintendenciaWebserviceHelper(_superintendenciaWebserviceConfiguration, _logger, _unitOfWork);
    //        string ticket = ws.Autenticacion();

    //        var listWS = ws.Provincias(ticket);

    //        foreach (var entidadWS in listWS)
    //        {

    //            var currentEntity = _unitOfWork.ProvinciaRepository.GetByIdSRT(entidadWS.ID);

    //            if (currentEntity == null)
    //            {
    //                currentEntity = new Provincia();
    //                currentEntity.Descripcion = entidadWS.Nombre;
    //                currentEntity.IdSRT = entidadWS.ID;
    //                _unitOfWork.ProvinciaRepository.Add(currentEntity);
    //            }
    //            else
    //            {                        
    //                if (currentEntity.Descripcion != entidadWS.Nombre)
    //                {
    //                    currentEntity.Descripcion = entidadWS.Nombre;
    //                    _unitOfWork.ProvinciaRepository.Upsert(currentEntity);
    //                }                        
    //            }                    
    //        }

    //        var provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled();

    //        foreach (var p in provincias)
    //        {
    //            if (listWS.FirstOrDefault(x=>x.ID == p.IdSRT) == null)
    //            {
    //                p.Disabled = true;
    //                _unitOfWork.ProvinciaRepository.Upsert(p);
    //            }
    //        }

    //        _unitOfWork.Save();

    //        return JsonOk("Cantidad", listWS.Count);

    //    }
    //    //catch (MedveValidationException ex)
    //    //{
    //    //    return JsonError("Se ha producido un error al eliminar el registro. Verfique que el mismo no tenga relaciones asociadas.");
    //    //}
    //    catch (Exception ex)
    //    {
    //        return JsonError(ex.Message);
    //        //return JsonError("Se ha producido un error al eliminar el registro. Verfique que el mismo no tenga relaciones asociadas.");
    //    }
    //}

  }
}

