using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Domain.Models;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Domain.Base;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OdontoSoft.Web.Controllers
{
  public partial class PacienteController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Paciente entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      //SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled(entity.ProvinciaId ?? 0).ToList(), "Id", "GetString", entity.ProvinciaId);
      //viewBagData.Add("provincia", provinciaList);

      //SelectList localidadList = new SelectList(_unitOfWork.LocalidadRepository.GetAllEnabled(entity.LocalidadId ?? 0).ToList(), "Id", "GetString", entity.LocalidadId);
      //viewBagData.Add("localidad", localidadList);

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Paciente entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Paciente entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Paciente entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Paciente entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
    [HttpPost]
    public async Task<IActionResult> Edit(PacienteDto vm)
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
        Paciente paciente;
        if (vm.Id != 0)
        {
          paciente = _unitOfWork.PacienteRepository.GetById(vm.Id);
        }
        else
        {
          paciente = new Paciente();
        }

        vm.MapDtoToPaciente(paciente);
        if (vm.Id != 0)
        {
          _unitOfWork.PacienteRepository.Upsert(paciente);
        }
        else
        {
          _unitOfWork.PacienteRepository.Add(paciente);
        }

        _unitOfWork.Save();

        return RedirectToAction("Paciente", "Browser");
      }
      return View(vm);
    }

    [HttpGet]
    public IActionResult Edit(int id = 0)
    {
      PacienteDto vm;

      if (id == 0)
      {
        vm = new PacienteDto();
      }
      else
      {
        var paciente = _unitOfWork.PacienteRepository.GetById(id);
        vm = new PacienteDto(paciente);
      }

      SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.ProvinciaId);
      SelectList localidadList = new SelectList(_unitOfWork.LocalidadRepository.GetAllEnabled().ToList(), "Id", "GetString", vm.LocalidadId);
      ViewBag.Provincia = provinciaList;
      ViewBag.Localidad = localidadList;
      return View(vm);
    }

    [HttpGet]
    public JsonResult GetAllByNombreDni(string searchText)
    {
      try
      {
        List<PacienteDto> lista = _unitOfWork.StoredProcedureRepository.GetPacienteForTurno(searchText).ToList();

        return Json(lista);
      }
      catch (Exception e)
      {
        return JsonError("Error al buscar los pacientes. " + e);
      }
    }

    [HttpGet]
    public JsonResult GetNombreById(int id)
    {
      try
      {
        var paciente = _unitOfWork.PacienteRepository.GetById(id);
        if (paciente == null) return JsonError("Paciente no encontrado");

        return Json(new
        {
          NombreCompleto = paciente.Apellido + ", " + paciente.Nombre
        });
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }
  }
}
