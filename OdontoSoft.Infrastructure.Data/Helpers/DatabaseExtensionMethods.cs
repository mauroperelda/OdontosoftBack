using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OdontoSoft.Infrastructure.Data.Helpers
{
  public static class DataRecordExtensions
  {
    public static bool HasColumn(this IDataRecord dr, string columnName)
    {
      for (int i = 0; i < dr.FieldCount; i++)
      {
        if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }
  }

  public static class DatabaseExtensionMethods
  {
    private static IList<T> MapToList<T>(this DbDataReader dr)
    {
      var objList = new List<T>();
      var props = typeof(T).GetRuntimeProperties();

      var colMapping = dr.GetColumnSchema()
          .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
          .ToDictionary(key => key.ColumnName.ToLower());

      if (dr.HasRows)
      {
        while (dr.Read())
        {
          T obj = Activator.CreateInstance<T>();
          foreach (var prop in props)
          {
            if (dr.HasColumn(prop.Name.ToLower()))
            {
              var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
              prop.SetValue(obj, val == DBNull.Value ? null : val);
            }
          }
          objList.Add(obj);
        }
      }
      return objList;
    }
    //public static bool HasColumn(DbDataReader Reader, string ColumnName)
    //{
    //    foreach (DataRow row in Reader.GetSchemaTable().Rows)
    //    {
    //        if (row["ColumnName"].ToString() == ColumnName)
    //            return true;
    //    } //Still here? Column not found. 
    //    return false;
    //}
    public static DbCommand LoadStoredProc(this DbContext context, string storedProcName)
    {
      var cmd = context.Database.GetDbConnection().CreateCommand();
      cmd.CommandText = storedProcName;
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      return cmd;
    }

    public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue)
    {
      if (string.IsNullOrEmpty(cmd.CommandText))
        throw new InvalidOperationException("Call LoadStoredProc before using this method");

      var param = cmd.CreateParameter();
      param.ParameterName = paramName;
      param.Value = paramValue;
      cmd.Parameters.Add(param);
      return cmd;
    }

    public static IList<T> ExecuteStoredProc<T>(this DbCommand command)
    {
      using (command)
      {
        if (command.Connection.State == System.Data.ConnectionState.Closed)
          command.Connection.Open();
        try
        {
          using (var reader = command.ExecuteReader())
          {
            return reader.MapToList<T>();
          }
        }
        finally
        {
          command.Connection.Close();
        }
      }
    }

    public static DataTable ExecuteStoredProc(this DbCommand command)
    {

      using (command)
      {
        if (command.Connection.State == System.Data.ConnectionState.Closed)
          command.Connection.Open();
        try
        {
          using (var reader = command.ExecuteReader())
          {
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
          }
        }
        finally
        {
          command.Connection.Close();
        }
      }

      //{
      //    DataTable table;
      //    try
      //    {
      //        string returnValue = string.Empty;
      //        DB = Connect();
      //        DBCommand = connection.Procedure("getCustomer");
      //        DB.AddInParameter(DBCommand, "@CustomerRef", DbType.String, b.CustomerRef1);

      //        DbDataReader reader = DBCommand.ExecuteReader();
      //        table = new DataTable();
      //        table.Load(reader);
      //        return table;
      //    }
      //    catch (Exception ex)
      //    {
      //        throw (ex);
      //    }

      //}

    }
  }
}
