using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Web.Controllers
{
  public partial class EstadoTurnoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(EstadoTurno entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      return viewBagData;
    }

    private void PrepareEntityFromFormData(EstadoTurno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
    }

    private void PrepareEntityBeforeLoadForm(EstadoTurno entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
    }

    private void ActionsBeforeSave(EstadoTurno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(EstadoTurno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }

    [HttpGet]
    public IActionResult Edit(int id = 0)
    {
      EstadoTurnoDto vm;

      if (id != 0)
      {
        var entity = _unitOfWork.EstadoTurnoRepository.GetById(id);
        vm = new EstadoTurnoDto(entity);
      }
      else
      {
        vm = new EstadoTurnoDto();
        vm.ColorHex = "#007bff"; // color default
      }

      return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(EstadoTurnoDto vm)
    {
      if (string.IsNullOrEmpty(vm.Nombre))
        ModelState.AddModelError(string.Empty, "El campo nombre no puede estar vacío.");
      if (string.IsNullOrEmpty(vm.ColorHex))
        ModelState.AddModelError(string.Empty, "Seleccioná un color.");

      if (ModelState.IsValid)
      {
        try
        {
          EstadoTurno entity;
          if (vm.Id != 0)
            entity = _unitOfWork.EstadoTurnoRepository.GetById(vm.Id);
          else
            entity = new EstadoTurno();

          vm.MapDtoToEstadoTurno(entity);

          if (vm.Id != 0)
            _unitOfWork.EstadoTurnoRepository.Upsert(entity);
          else
            _unitOfWork.EstadoTurnoRepository.Add(entity);

          _unitOfWork.Save();

          return RedirectToAction("Index", "Browser", new { code = "EstadoTurno" });
        }
        catch (Exception ex)
        {
          ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
        }
      }

      return View(vm);
    }

    // Usado por el calendario para cargar los estados en el selector de color
    [HttpGet]
    public JsonResult GetEstados()
    {
      try
      {
        var estados = _unitOfWork.EstadoTurnoRepository
            .GetAllEnabled()
            .Select(e => new { e.Id, e.Nombre, e.ColorHex })
            .ToList();
        return JsonOk("estados", estados);
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }
  }
}

