using OdontoSoft.Data.DAL;
//using OdontoSoft.Data.Helpers;
using OdontoSoft.Data.Repository.Interfaces;
//using OdontoSoft.Entities.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdontoSoft.Entities.ViewModels;
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

        public IList<PacienteViewModel> GetPacienteForTurno(string searchText)
        {
            IList<PacienteViewModel> r = _context.LoadStoredProc("SP_GetPacientesForTurno")
                                .WithSqlParam("@busqueda", searchText)
                                .ExecuteStoredProc<PacienteViewModel>();
            return r.ToList();
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
