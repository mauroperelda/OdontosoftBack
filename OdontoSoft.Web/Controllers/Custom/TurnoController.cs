using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Domain.Models;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Domain.Base;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace OdontoSoft.Web.Controllers
{
  public partial class TurnoController : AppBaseBrowserController
  {
    private Dictionary<string, object> PrepareViewBag(Turno entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Turno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Turno entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Turno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Turno entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }
    [HttpGet]
    public IActionResult EditPartial(int id = 0, string fecha = null)
    {
      var vm = new TurnoDto();

      if (id != 0)
      {
        var turno = _unitOfWork.TurnoRepository.GetByTurnoId(id);
        if (turno != null)
          vm = new TurnoDto(turno);
      }

      // Si viene fecha desde el click del calendario, pre-cargarla
      if (!string.IsNullOrEmpty(fecha) && DateTime.TryParseExact(fecha, "dd/MM/yyyy",
          System.Globalization.CultureInfo.InvariantCulture,
          System.Globalization.DateTimeStyles.None, out var fechaParsed))
      {
        vm.Fecha = fechaParsed;
      }

      return PartialView("_PartialShift", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TurnoDto vm, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      if (vm.PacienteId == 0) { ModelState.AddModelError(string.Empty, "El campo paciente no puede estar vacio."); }
      if (vm.ProfesionalId == 0) { ModelState.AddModelError(string.Empty, "El campo profesional no puede estar vacio."); }
      if (vm.Fecha == null) { ModelState.AddModelError(string.Empty, "El campo fecha no puede estar vacio."); }
      if (vm.HoraInicio == null) { ModelState.AddModelError(string.Empty, "El campo fecha de inicio no puede estar vacio"); }

      if (!ModelState.IsValid)
      {
        var errores = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return Json(new { success = false, errors = errores });
      }

      Turno turno;
      if (vm.Id != 0)
      {
        turno = _unitOfWork.TurnoRepository.GetByTurnoId(vm.Id);
      }
      else
      {
        var currentModel = _unitOfWork.TurnoRepository.GetByProfesionalByHoraInicio(vm.ProfesionalId, vm.HoraInicio);
        if (currentModel != null)
          return Json(new { success = false, errors = new[] { "El profesional ya tiene agendado un turno en esa hora." } });

        turno = new Turno();
      }

      vm.MapDtoToTurno(turno);
      if (vm.Id != 0)
        _unitOfWork.TurnoRepository.Upsert(turno);
      else
        _unitOfWork.TurnoRepository.Add(turno);

      _unitOfWork.Save();

      return Json(new { success = true });
    }

    public JsonResult ListarTurnosCalendario()
    {
      try
      {
        List<Turno> turnos = new List<Turno>();
        List<TurnoDto> turnoDtoList = new List<TurnoDto>();
        turnos = _unitOfWork.TurnoRepository.GetAll().ToList();
        if (turnos != null)
        {
          foreach (var item in turnos)
          {
            var vm = new TurnoDto
            {
              Id = item.Id,
              PacienteId = item.PacienteId,
              Paciente = item.Paciente.Nombre + " " + item.Paciente.Apellido,
              ProfesionalId = item.ProfesionalId,
              Profesional = item.Profesional.Nombre + " " + item.Profesional.Apellido,
              Fecha = item.Fecha,
              HoraInicio = item.HoraInicio,
              HoraFin = item.HoraFin,
              Observacion = item.Observacion,
              TratamientoId = item.TratamientoId,
              Dni = item.Paciente.Dni,
              ColorEstado = item.EstadoTurno?.ColorHex ?? "#007bff"
            };
            turnoDtoList.Add(vm);
          }
        }
        return Json(turnoDtoList);
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }
  }
}
