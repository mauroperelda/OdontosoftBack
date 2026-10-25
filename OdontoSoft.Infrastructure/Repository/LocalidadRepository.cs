using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class LocalidadRepository : GenericRepository<Localidad>, ILocalidadRepository
  {
    /*Ejemplo de reemplazo de mÃ©todos existentes*/

    //public override Localidad GetById(int id)
    //{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}

    //Ejemplo para modificar la lista que llena el browser
    //public override IQueryable<Localidad> AllForBrowser()
    //{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x => x.Descripcion);
    //}


    /*Ejemplo de nuevos mÃ©todos - Recuerde ademas modificar la interfaz ILocalidadRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //public Localidad GetByMiCampo(int miCampoId)
    //{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}

    //Ejemplo custom de getBy con una lista de resultado
    //public IQueryable<Localidad> GetByMiOtroCampo(int miOtroCampoId)
    //{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}

    public Localidad GetById(int id)
    {
      return All().FirstOrDefault(x => x.Id == id);
    }

    public override IQueryable<Localidad> AllForBrowser()
    {
      return base.AllForBrowser()
          .Include(x => x.Provincia)
          .OrderBy(x => x.Descripcion);
    }

    public IQueryable<Localidad> GetByProvinciaId(int provinciaId)
    {
      return All().Include(x => x.Provincia).Where(x => x.ProvinciaId == provinciaId).OrderBy(x => x.Descripcion);
    }

    /// <summary>
    /// Inserción masiva con el COPY binario de PostgreSQL.
    ///
    /// La tabla 'localidad' no tiene campos de auditoría (sólo id, descripcion,
    /// provincia_id y disabled), así que no hace falta pasar por EF. El id lo
    /// asigna la secuencia identity de la tabla.
    ///
    /// Antes esta operación usaba SqlBulkCopy contra 'dbo.Localidad', que es
    /// código de SQL Server y no funciona contra PostgreSQL.
    /// </summary>
    public int BulkInsert(IEnumerable<Localidad> localidades)
    {
      var lista = localidades?.ToList() ?? new List<Localidad>();
      if (lista.Count == 0) return 0;

      var conexion = _context.Database.GetDbConnection() as NpgsqlConnection;
      if (conexion == null)
      {
        throw new InvalidOperationException(
          "La conexión actual no es de PostgreSQL: la inserción masiva de localidades requiere Npgsql.");
      }

      // OpenConnection/CloseConnection son transaction-aware: si EF ya abrió
      // la conexión por una transacción, CloseConnection no la cierra.
      _context.Database.OpenConnection();
      try
      {
        using var importer = conexion.BeginBinaryImport(
          "COPY localidad (descripcion, provincia_id, disabled) FROM STDIN (FORMAT BINARY)");

        foreach (var l in lista)
        {
          importer.StartRow();
          importer.Write(l.Descripcion ?? string.Empty, NpgsqlDbType.Text);
          importer.Write(l.ProvinciaId, NpgsqlDbType.Integer);
          importer.Write(l.Disabled, NpgsqlDbType.Boolean);
        }

        // Complete() cierra el COPY y hace que los datos queden en la tabla.
        // Va una sola vez, al final: llamarlo dentro del loop cerraría la
        // copia en la primera fila ("The COPY operation has already been
        // committed") y dejaría el resto sin insertar.
        importer.Complete();

        return lista.Count;
      }
      finally
      {
        _context.Database.CloseConnection();
      }
    }
  }
}

