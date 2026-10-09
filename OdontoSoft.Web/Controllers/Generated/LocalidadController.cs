using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Web.Helpers.Authentication;
using OdontoSoft.Web.Browser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OdontoSoft.Web.Controllers
{
    public partial class LocalidadController : AppBaseBrowserController
    {
        private int _entityId;

        public ActionResult EditForm(int id = 0)
        {
            if (!AllowAction(id)) return Unauthorized();
            Localidad entity;
            if (id != 0)
                entity = _unitOfWork.LocalidadRepository.GetById(id);
            else
                entity = new Localidad();
            PrepareEntityBeforeLoadForm(entity);
            base.LoadViewBag(PrepareViewBag(entity));
            return View(entity);
        }

        [HttpPost]
        public ActionResult EditForm(Localidad entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
        {
            if (!AllowAction(entity.Id)) return Unauthorized();
            if (ModelState.IsValid)
            {
                try
                {
                    Localidad currentModel = null;
                    if (entity.Id != 0)
                    {
                        currentModel = _unitOfWork.LocalidadRepository.GetById(entity.Id);
                        TryUpdateModelAsync(currentModel);
                    }
                    else
                        currentModel = entity;
                    PrepareEntityFromFormData(currentModel, formCollection);
                    ActionsBeforeSave(currentModel, formCollection);
                    if (entity.Id != 0)
                        _unitOfWork.LocalidadRepository.Upsert(currentModel);
                    else
                        _unitOfWork.LocalidadRepository.Add(currentModel);
                    _unitOfWork.Save();
                    _entityId = currentModel.Id;
                    ModelState.SetModelValue("Id", new ValueProviderResult(currentModel.Id.ToString()));
                    entity = _unitOfWork.LocalidadRepository.GetById(_entityId);
                    ActionsAfterSave(currentModel, formCollection);
                    return RedirectToBrowser(formCollection);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        if (ex.InnerException.InnerException != null)
                            ModelState.AddModelError(string.Empty, ex.InnerException.InnerException.Message);
                        else
                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    else
                        ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            PrepareEntityFromFormData(entity, formCollection);
            base.LoadViewBag(PrepareViewBag(entity));
            return View(entity);
        }

        public JsonResult DeleteAjax(int id)
        {
            try
            {
                _unitOfWork.LocalidadRepository.Delete(id);
                _unitOfWork.Save();
                return JsonOk();
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }
    }
}

