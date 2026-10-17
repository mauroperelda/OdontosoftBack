//using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Contracts.Dtos;
using System.Data;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IStoredProcedureRepository
  {
    //IList<ServicioItemReferenciaViewModel> ReferenciaCodigoValorListByCodigo(string storedProcedureName, string codigoReferencia, string searchText);
    //IList<ResultadoBusquedaItemViewModel> GeneralSearch(string searchText);
    //IList<ServicioGridItemViewModel> ProximosServicios(DateTime fechaHasta, int sucursalId);
    //DataTable ExportServicios(int clienteId, int vehiculoId, int servicioTipoid, int sucursalId, DateTime? fechaDesde, DateTime? fechaHasta, string dominio, string comprobanteInterno);

    IList<PacienteDto> GetPacienteForTurno(string searchText);

  }
}
