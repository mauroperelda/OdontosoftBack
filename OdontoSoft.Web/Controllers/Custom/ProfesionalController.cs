using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Entities.Models;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Entities.Base;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OdontoSoft.Web.Controllers
{
  public partial class ProfesionalController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Profesional entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      //SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled(entity.ProvinciaId ?? 0).ToList(), "Id", "GetString", entity.ProvinciaId);
      //viewBagData.Add("provincia", provinciaList);

      //SelectList localidadList = new SelectList(_unitOfWork.LocalidadRepository.GetAllEnabled(entity.LocalidadId ?? 0).ToList(), "Id", "GetString", entity.LocalidadId);
      //viewBagData.Add("localidad", localidadList);

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Profesional entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Profesional entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Profesional entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Profesional entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
    [HttpGet]
    public IActionResult Edit(int id = 0)
    {
      ProfesionalDto vm;

      if (id == 0)
      {
        vm = new ProfesionalDto();
      }
      else
      {
        var profesional = _unitOfWork.ProfesionalRepository.GetById(id);
        vm = new ProfesionalDto(profesional);
      }

      SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.ProvinciaId);
      SelectList localidadList = new SelectList(_unitOfWork.LocalidadRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.LocalidadId);
      ViewBag.Provincia = provinciaList;
      ViewBag.Localidad = localidadList;
      return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(ProfesionalDto vm)
    {
      if (string.IsNullOrEmpty(vm.Nombre)) { ModelState.AddModelError(string.Empty, "El campo nombre no puede estar vacio."); }
      if (string.IsNullOrEmpty(vm.Apellido)) { ModelState.AddModelError(string.Empty, "El campo apellido no puede estar vacio."); }
      if (string.IsNullOrEmpty(vm.Dni)) { ModelState.AddModelError(string.Empty, "El campo dni no puede estar vacio."); }
      if (vm.FechaNacimiento == null) { ModelState.AddModelError(string.Empty, "El campo fecha de nacimiento no puede estar vacio."); }
      if (vm.ProvinciaId == null) { ModelState.AddModelError(string.Empty, "El campo provincia no puede estar vacio."); }
      if (vm.LocalidadId == null) { ModelState.AddModelError(string.Empty, "El campo localidad no puede estar vacio."); }

      SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.ProvinciaId);
      SelectList localidadList = new SelectList(_unitOfWork.LocalidadRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.LocalidadId);
      ViewBag.Provincia = provinciaList;
      ViewBag.Localidad = localidadList;

      if (ModelState.IsValid)
      {
        Profesional profesional;
        if (vm.Id != 0)
        {
          profesional = _unitOfWork.ProfesionalRepository.GetById(vm.Id);
        }
        else
        {
          profesional = new Profesional();
        }

        vm.MapDtoToProfesional(profesional);
        if (vm.Id != 0)
        {
          _unitOfWork.ProfesionalRepository.Upsert(profesional);
        }
        else
        {
          _unitOfWork.ProfesionalRepository.Add(profesional);
        }

        _unitOfWork.Save();

        return RedirectToAction("Profesional", "Browser");
      }
      return View(vm);
    }

    [HttpGet]
    public JsonResult GetProfesionales()
    {
      try
      {
        var profesionales = _unitOfWork.ProfesionalRepository.GetAllEnabled().ToList();
        return JsonOk("profesionales", profesionales);
      }
      catch (Exception ex)
      {
        return JsonError("Error al cargar los profesionales " + ex.Message);
      }
    }
  }
}
