using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Entities.ViewModels;

namespace OdontoSoft.Web.Controllers
{
	public partial class HistoriaClinicaController : AppBaseBrowserController
	{
		private Dictionary<string, object> PrepareViewBag(HistoriaClinica entity)
		{
			Dictionary<string, object> viewBagData = new Dictionary<string, object>();
			SelectList pacienteList = new SelectList(_unitOfWork.PacienteRepository.GetAllEnabled(entity.PacienteId).ToList(), "Id", "GetString", entity.PacienteId);
			viewBagData.Add("paciente", pacienteList);
			return viewBagData;
		}

		private void PrepareEntityFromFormData(HistoriaClinica entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
		{
			//Implement here any functionallity to update the entity from specific data in formCollection.
		}

		private void PrepareEntityBeforeLoadForm(HistoriaClinica entity)
		{
			//Implement here any functionallity to update the entity before EditForm shows.
		}

		private void ActionsBeforeSave(HistoriaClinica entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
		{
			//Implement here any functionallity to execute any action before unitofworks.Save
		}

		private void ActionsAfterSave(HistoriaClinica entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
		{
			//Implement here any functionallity to execute any action after unitofworks.Save
		}

		[HttpGet]
		public IActionResult Edit(int id = 0, int pacienteId = 0)
		{
			HistoriaClinicaViewModel vm;
			HistoriaClinica hc = null;

			if (id != 0)
			{
				// ✅ Primero intentar buscar por Id directo de HistoriaClinica
				hc = _unitOfWork.HistoriaClinicaRepository.GetHistoriaClinicaById(id);
			}
			else if (pacienteId != 0)
			{
				hc = _unitOfWork.HistoriaClinicaRepository.GetFullByPacienteId(pacienteId);
			}

			if (hc != null)
			{
				vm = new HistoriaClinicaViewModel(hc);
			}
			else
			{
				vm = new HistoriaClinicaViewModel();
			}

			CargarViewBags(vm);
			return View(vm);
		}

		[HttpPost]
		public IActionResult Edit(HistoriaClinicaViewModel vm)
		{
			if (vm.PacienteId == 0)
				ModelState.AddModelError(string.Empty, "El campo paciente no puede estar vacío.");

			CargarViewBags(vm);

			if (ModelState.IsValid)
			{
				try
				{
					HistoriaClinica hc;
					if (vm.Id != 0)
					{
						hc = _unitOfWork.HistoriaClinicaRepository.GetById(vm.Id);
					}
					else
					{
						var existente = _unitOfWork.HistoriaClinicaRepository.GetByPacienteId(vm.PacienteId);
						if (existente != null)
						{
							ModelState.AddModelError(string.Empty, "El paciente ya tiene una historia clínica creada.");
							return View(vm);
						}
						hc = new HistoriaClinica();
					}

					vm.MapViewModelToHistoriaClinica(hc);

					if (vm.Id != 0)
						_unitOfWork.HistoriaClinicaRepository.Upsert(hc);
					else
						_unitOfWork.HistoriaClinicaRepository.Add(hc);

					_unitOfWork.Save();

					return RedirectToAction("Edit", "HistoriaClinica", new { id = hc.Id, pacienteId = hc.PacienteId});
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
				}
			}

			return View(vm);
		}

		// Buscar HC por paciente desde el browser o desde turno
		[HttpGet]
		public IActionResult EditByPaciente(int id)
		{
			var hc = _unitOfWork.HistoriaClinicaRepository.GetByPacienteId(id);
			if (hc != null)
				return RedirectToAction("Edit", new { id = hc.Id, pacienteId = id });
			else
				return RedirectToAction("Edit", new { pacienteId = id });
		}

		// Guardar/actualizar una pieza del odontograma via AJAX
		[HttpPost]
		public JsonResult GuardarPieza([FromBody] OdontogramaPiezaViewModel vm)
		{
			try
			{
				var pieza = _unitOfWork.OdontogramaPiezaRepository
						.GetByHistoriaClinicaIdAndNumeroPieza(vm.HistoriaClinicaId, vm.NumeroPieza);

				if (pieza != null)
				{
					vm.Id = pieza.Id;
					vm.MapViewModelToOdontogramaPieza(pieza);
					_unitOfWork.OdontogramaPiezaRepository.Upsert(pieza);
				}
				else
				{
					pieza = new OdontogramaPieza();
					vm.MapViewModelToOdontogramaPieza(pieza);
					_unitOfWork.OdontogramaPiezaRepository.Add(pieza);
				}

				_unitOfWork.Save();
				return JsonOk();
			}
			catch (Exception ex)
			{
				return JsonError(ex.Message);
			}
		}

		// Guardar ficha de tratamiento via AJAX
		[HttpPost]
		public JsonResult GuardarFicha([FromBody] FichaTratamientoViewModel vm)
		{
			try
			{
				FichaTratamiento ficha;
				if (vm.Id != 0)
				{
					ficha = _unitOfWork.FichaTratamientoRepository.GetFichaTratamientoById(vm.Id);
				}
				else
				{
					ficha = new FichaTratamiento();
					ficha.FechaRealizacion = DateTime.Now;
				}

				vm.MapViewModelToFichaTratamiento(ficha);

				if (vm.Id != 0)
					_unitOfWork.FichaTratamientoRepository.Upsert(ficha);
				else
					_unitOfWork.FichaTratamientoRepository.Add(ficha);

				_unitOfWork.Save();

				return JsonOk("ficha", new
				{
					ficha.Id,
					Tratamiento = ficha.Tratamiento?.Nombre,
					ficha.NumeroPieza,
					ficha.Descripcion,
					FechaRealizacion = ficha.FechaRealizacion.ToString("dd/MM/yyyy")
				});
			}
			catch (Exception ex)
			{
				return JsonError(ex.Message);
			}
		}

		// Eliminar ficha via AJAX
		[HttpPost]
		public JsonResult EliminarFicha(int id)
		{
			try
			{
				_unitOfWork.FichaTratamientoRepository.Delete(id);
				_unitOfWork.Save();
				return JsonOk();
			}
			catch (Exception ex)
			{
				return JsonError(ex.Message);
			}
		}

		// Obtener tratamientos para el select de fichas
		[HttpGet]
		public JsonResult GetTratamientosParaFicha()
		{
			try
			{
				var tratamientos = _unitOfWork.TratamientoRepository
						.GetAllEnabled()
						.Select(t => new { t.Id, t.Nombre })
						.ToList();
				return JsonOk("tratamientos", tratamientos);
			}
			catch (Exception ex)
			{
				return JsonError(ex.Message);
			}
		}
		private void CargarViewBags(HistoriaClinicaViewModel vm)
		{
			ViewBag.Pacientes = new SelectList(
					_unitOfWork.PacienteRepository.GetAllEnabled().ToList(),
					"Id", "GetString", vm.PacienteId);
		}
	}
}

