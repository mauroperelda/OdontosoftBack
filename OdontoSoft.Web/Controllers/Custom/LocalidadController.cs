using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Web.Helpers;
using Microsoft.VisualStudio.TextTemplating;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace OdontoSoft.Web.Controllers
{
  public partial class LocalidadController : AppBaseBrowserController
  {

    private Dictionary<string, object> PrepareViewBag(Localidad entity)
    {
      Dictionary<string, object> viewBagData = new Dictionary<string, object>();
      //viewBagData.Add("myKey", "myValue");

      SelectList provinciaList = new SelectList(_unitOfWork.ProvinciaRepository.GetAllEnabled().ToList(), "Id", "GetString", entity.ProvinciaId);
      viewBagData.Add("provincia", provinciaList);

      return viewBagData;
    }


    private void PrepareEntityFromFormData(Localidad entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to update the entity from specific data in formCollection.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/5636097/Ejemplo+de+uso+de+PrepareEntityFromFormData
    }

    private void PrepareEntityBeforeLoadForm(Localidad entity)
    {
      //Implement here any functionallity to update the entity before EditForm shows.
      //Sample: https://medve.atlassian.net/wiki/spaces/mdv/pages/10747905/Ejemplo+de+uso+de+PrepareEntityBeforeLoadForm
    }

    private void ActionsBeforeSave(Localidad entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action before unitofworks.Save
    }

    private void ActionsAfterSave(Localidad entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      //Implement here any functionallity to execute any action after unitofworks.Save
    }

    public JsonResult GetByProvincia(int provinciaId)
    {
      try
      {
        var localidades = _unitOfWork.LocalidadRepository.GetByProvinciaId(provinciaId).ToList();
        return JsonOk("localidades", localidades);
      }
      catch (Exception ex)
      {
        return JsonError("Error al cargar las localidades " + ex.Message);
      }
    }
    [HttpGet]
    public IActionResult LocalidadImport()
    {
      return View();
    }

    [HttpGet]
    public async Task<JsonResult> SincronizarLocalidades()
    {
      GeorefResponseDto georefResponseDto = new GeorefResponseDto();
      try
      {
        georefResponseDto = await _localidadServices.SincronizarLocalidades();
        //Inserto localidades a la DB
        BulkInsertLocalidades(georefResponseDto);
        return JsonOk();
      }
      catch (Exception ex)
      {
        return JsonError(ex.Message);
      }
    }

    private void BulkInsertLocalidades(GeorefResponseDto georefResponseDto)
    {
      try
      {
        List<Provincia> provincias = _unitOfWork.ProvinciaRepository.GetAllEnabled().ToList();

        var provinciasDict = provincias
            .ToDictionary(
                p => NormalizeTextHelper.NormalizeText(p.Descripcion),
                p => p.Id

            );

        // Localidades existentes en DB
        List<Localidad> localidadesExistentes = _unitOfWork.LocalidadRepository.GetAllEnabled().ToList();
        var localidadesExistentesSet = new HashSet<string>(
            localidadesExistentes.Select(l =>
                NormalizeTextHelper.NormalizeText(l.Descripcion) + "|" + l.ProvinciaId
            )
        );

        List<Localidad> localidadesAInsertar = new List<Localidad>();

        foreach (var loc in georefResponseDto.localidades)
        {
          string provinciaNombreNormalizado = NormalizeTextHelper.NormalizeText(loc.provincia.nombre);

          if (provinciasDict.TryGetValue(provinciaNombreNormalizado, out int provinciaId))
          {
            string key = NormalizeTextHelper.NormalizeText(loc.nombre) + "|" + provinciaId;

            if (!localidadesExistentesSet.Contains(key))
            {
              localidadesAInsertar.Add(new Localidad
              {
                Descripcion = loc.nombre,
                ProvinciaId = provinciaId,
                Disabled = false,
                ObjectState = ObjectState.Added
              });

              // También la agrego al HashSet para evitar duplicados dentro del mismo JSON
              localidadesExistentesSet.Add(key);
            }
          }
          else
          {
            // Podés loguear las que no encontró
            Console.WriteLine($"Provincia no encontrada: {loc.provincia.nombre}");
          }
        }

        // 👉 Ahora hacemos el bulk insert con SqlBulkCopy
        if (localidadesAInsertar.Any())
        {
          var connectionString = _configuration.GetConnectionString("DefaultConnection");
          using (var bulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
          {
            bulkCopy.DestinationTableName = "dbo.Localidad";

            // Mapear columnas
            bulkCopy.ColumnMappings.Add("Descripcion", "Descripcion");
            bulkCopy.ColumnMappings.Add("ProvinciaId", "ProvinciaId");
            bulkCopy.ColumnMappings.Add("Disabled", "Disabled");

            var dataTable = ToDataTable(localidadesAInsertar);
            bulkCopy.WriteToServer(dataTable);
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private DataTable ToDataTable(List<Localidad> localidades)
    {
      var table = new DataTable();
      table.Columns.Add("Descripcion", typeof(string));
      table.Columns.Add("ProvinciaId", typeof(int));
      table.Columns.Add("Disabled", typeof(bool));

      foreach (var loc in localidades)
      {
        table.Rows.Add(loc.Descripcion, loc.ProvinciaId, loc.Disabled);
      }

      return table;
    }

  }
}

