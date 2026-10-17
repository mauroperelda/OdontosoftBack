using OdontoSoft.Data.DAL;
//using OdontoSoft.Data.Helpers;
using OdontoSoft.Data.Repository.Interfaces;
//using OdontoSoft.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Data.Helpers;

namespace OdontoSoft.Data.Repository.StoredProcedure
{
  public class StoredProcedureRepository : IStoredProcedureRepository
  {

    //private const string GENERAL_SEARCH_SP_NAME = "SP_SEARCH_ALL";
    //private const string EXPORT_SERVICIOS_SP_NAME = "SP_EXPORT_SERVICIOS";
    //private const string PROXIMOS_SERVICIOS_SP_NAME = "SP_PROXIMOS_SERVICIOS";

    protected OdontosoftContext _context;
    protected readonly ILogger _logger;

    public StoredProcedureRepository(OdontosoftContext context, ILogger logger)
    {
      _context = context;
      _logger = logger;
    }

    public IList<PacienteDto> GetPacienteForTurno(string searchText)
    {
      // Antes: stored procedure SQL Server `SP_GetPacientesForTurno` ejecutada
      // con un helper custom (LoadStoredProc / WithSqlParam / ExecuteStoredProc).
      // Ahora: LINQ directo contra `paciente` (tabla snake_case). Npgsql traduce
      // el Contains/ToLower a ILIKE en el SQL, y la projection a PacienteDto
      // se hace client-side con `Select`.
      //
      // La PL/pgSQL function `get_pacientes_for_turno` igual existe (ver migration
      // CreateGetPacientesForTurnoFunction) por si en el futuro queremos exponerla
      // a otros clientes (psql, un ETL, el front directo). No la usamos desde acá
      // porque EF Core 9 no soporta `Database.SqlQueryRaw<T>` para tipos complejos
      // (eso es EF Core 10).
      var query = _context.Paciente.AsNoTracking();

      if (!string.IsNullOrWhiteSpace(searchText))
      {
        var like = searchText.Trim().ToLower();
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(like)
            || p.Apellido.ToLower().Contains(like)
            || p.Dni.Contains(searchText.Trim()));
      }

      return query
          .Where(p => !p.Disabled)
          .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
          .Take(50)
          .Select(p => new PacienteDto
          {
            Id = p.Id,
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            Dni = p.Dni
          })
          .ToList();
    }

    //public IList<ServicioItemReferenciaViewModel> ReferenciaCodigoValorListByCodigo(string storedProcedureName, string codigoReferencia, string searchText)
    //{
    //    IList<ServicioItemReferenciaViewModel> r = _context.LoadStoredProc(storedProcedureName)
    //                .WithSqlParam("@codigoReferencia", codigoReferencia)
    //                .WithSqlParam("@searchText", searchText)
    //                .ExecuteStoredProc<ServicioItemReferenciaViewModel>();
    //    return r.ToList();

    //}

    //public IList<ResultadoBusquedaItemViewModel> GeneralSearch(string searchText)
    //{
    //    IList<ResultadoBusquedaItemViewModel> r = _context.LoadStoredProc(GENERAL_SEARCH_SP_NAME)
    //                .WithSqlParam("@searchText", searchText)
    //                .ExecuteStoredProc<ResultadoBusquedaItemViewModel>();
    //    return r.ToList();
    //}

    //public DataTable ExportServicios(int clienteId, int vehiculoId, int servicioTipoid, int sucursalId, DateTime? fechaDesde, DateTime? fechaHasta, string dominio, string comprobanteInterno)
    //{
    //    DataTable dt = _context.LoadStoredProc(EXPORT_SERVICIOS_SP_NAME)
    //        .WithSqlParam("@clienteId", clienteId)
    //        .WithSqlParam("@vehiculoId", vehiculoId)
    //        .WithSqlParam("@servicioTipoid", servicioTipoid)
    //        .WithSqlParam("@sucursalId", sucursalId)
    //        .WithSqlParam("@fechaDesde", fechaDesde == null ? DBNull.Value : fechaDesde)
    //        .WithSqlParam("@fechaHasta", fechaHasta == null ? DBNull.Value : fechaHasta)
    //        .WithSqlParam("@dominio", dominio ?? "")
    //        .WithSqlParam("@comprobanteInterno", comprobanteInterno??"")
    //        .ExecuteStoredProc();

    //    return dt;

    //}

    //public IList<ServicioGridItemViewModel> ProximosServicios(DateTime fechaHasta, int sucursalId)
    //{
    //    IList<ServicioGridItemViewModel> r = _context.LoadStoredProc(PROXIMOS_SERVICIOS_SP_NAME)
    //                .WithSqlParam("@fechaHasta", fechaHasta)
    //                .WithSqlParam("@sucursalId", sucursalId)
    //                .ExecuteStoredProc<ServicioGridItemViewModel>();
    //    return r.ToList();
    //}


  }
}
