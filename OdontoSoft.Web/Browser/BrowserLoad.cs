using OdontoSoft.Infrastructure.Repository;
using OdontoSoft.Application.Abstractions.Persistence;
using MedveCore.Common.Helpers;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace OdontoSoft.Web.Browser
{
  internal class BrowserLoader<TEntity> where TEntity : BaseEntity, IObjectState
  {

    private IQueryable<dynamic> GetData(HttpContext h)
    {
      var u = (OdontoSoft.Infrastructure.Repository.UOW.IUnitOfWork)h.RequestServices.GetService(typeof(OdontoSoft.Infrastructure.Repository.UOW.IUnitOfWork));
      IQueryable<dynamic> queryable = ((OdontoSoft.Infrastructure.Repository.UOW.IUnitOfWork)u).GetAll(typeof(TEntity));
      return queryable;
    }

    public void LoadBrowserDatasource(BrowserModel browserInstance, BrowserDatasourceParameters parameters, IHttpContextAccessor accessor)
    {


      IQueryable<dynamic> queryable = GetData(accessor.HttpContext);

      if (parameters != null)
      {
        #region Filter
        queryable = this.GetFilteredDatasource(queryable, parameters.Filter);
        #endregion

        #region Sort
        if (parameters.Sort == null || (parameters.Sort.Fields == null || parameters.Sort.Fields.Count < 1))
        {
          //ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
          //MemberExpression property;
          //if (parameters.Group != null && !string.IsNullOrEmpty(parameters.Group.DataField))
          //{
          //    property = this.GetPropertyOrField(parameter, parameters.Group.DataField);
          //}
          //else
          //{
          //    property = this.GetPropertyOrField(parameter, "Id");
          //}
          //Expression sort = Expression.Lambda(property, parameter);
          //Expression call = Expression.Call(
          //    typeof(Queryable),
          //    "OrderBy",
          //    new[] { typeof(TEntity), property.Type },
          //    queryable.Expression,
          //    Expression.Quote(sort));

          //queryable = (IOrderedQueryable<TEntity>)queryable.Provider.CreateQuery<TEntity>(call);
        }
        else
        {
          bool anotherLevel = false;
          foreach (BrowserDatasourceSortFieldParameter item in parameters.Sort.Fields)
          {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = this.GetPropertyOrField(parameter, item.Field);
            var sort = Expression.Lambda(property, parameter);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (item.Descending ? "Descending" : string.Empty),
                new[] { typeof(TEntity), property.Type },
                queryable.Expression,
                Expression.Quote(sort));

            queryable = (IOrderedQueryable<TEntity>)queryable.Provider.CreateQuery<TEntity>(call);
            if (anotherLevel) { anotherLevel = true; }
          }
        }
        #endregion

        #region Group
        if (parameters.Group != null)
        {
          if (parameters.Group.Fields != null && parameters.Group.Fields.Count > 0)
          {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            foreach (BrowserDatasourceGroupFieldParameter item in parameters.Group.Fields)
            {
              //var property = this.GetPropertyOrField(parameter, item.Field);
              //ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
              Expression property = this.GetPropertyOrField(parameter, item.Field);

              //MethodInfo objectToStringMethodInfo = typeof(object).GetMethod("ToString", System.Type.EmptyTypes);
              //property = Expression.Call(property, objectToStringMethodInfo);

              Expression groupBy = Expression.Lambda(property, parameter);
              Expression callGroupBy = Expression.Call(
                  typeof(Queryable),
                  "GroupBy",
                  new[] { queryable.ElementType, property.Type },
                  queryable.Expression,
                  Expression.Quote(groupBy));

              queryable = (IQueryable<dynamic>)queryable.Provider.CreateQuery(callGroupBy);
            }

            if (string.IsNullOrEmpty(parameters.Group.DataField))
            {
              queryable = ((IQueryable<IGrouping<dynamic, TEntity>>)queryable)
                  .Select(x => new
                  {
                    key = ((object)x.Key) == null ? null : ((object)x.Key),
                    count = x.Count(),
                    items = x.AsQueryable()
                  })
                  .OrderBy(x => x.key);

            }
            else
            {
              queryable = ((IQueryable<IGrouping<dynamic, TEntity>>)queryable)
                  .Select(x => new
                  {
                    key = ((object)x.Key) == null ? null : ((object)x.Key),
                    count = x.Count()
                  })
                  .OrderBy(x => x.key);
            }
          }
        }
        #endregion

        #region Skip
        queryable = queryable.Skip(parameters.Skip);
        #endregion

        #region Take
        if (parameters.Take > 0)
        {
          queryable = queryable.Take(parameters.Take);
        }
        #endregion
      }

      browserInstance.Datasource = queryable;
    }

    public int GetBrowserDatasourceTotalCount(BrowserDatasourceParameters parameters, IHttpContextAccessor accessor)
    {
      IQueryable<dynamic> queryable = GetData(accessor.HttpContext);

      if (parameters != null)
      {
        #region Filter
        queryable = this.GetFilteredDatasource(queryable, parameters.Filter);
        #endregion
      }

      return queryable.Count();
    }

    //private IQueryable<TEntity> GetBrowserDatasource(HttpContext h)
    //{
    //    IGenericRepository<TEntity> service = h.RequestServices.GetService<IGenericRepository<TEntity>>();
    //    return service.All();
    //}

    private MemberExpression GetPropertyOrField(Expression parameter, string field)
    {
      if (field.Contains("."))
      {
        Expression property = parameter;
        foreach (string expr in field.Split('.'))
        {
          property = Expression.PropertyOrField(property, expr);
        }
        return (MemberExpression)property;
      }
      else
      {
        return Expression.PropertyOrField(parameter, field);
      }
    }

    private IQueryable<dynamic> GetFilteredDatasource(IQueryable<dynamic> queryable, BrowserDatasourceFilterParameter filter)
    {
      IQueryable<dynamic> filteredQueryable = queryable;
      if (filter != null && filter.Items != null && filter.Items.Count > 0)
      {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression filterResult = this.FilterExpressionBuilder(filter.Items, parameter);
        Expression where = Expression.Lambda<Func<TEntity, bool>>(filterResult, new ParameterExpression[] { parameter });

        MethodCallExpression whereCallExpression = Expression.Call(
            typeof(Queryable),
            "Where",
            new Type[] { queryable.ElementType },
            queryable.Expression,
            where);

        filteredQueryable = queryable.Provider.CreateQuery<TEntity>(whereCallExpression);
      }

      return filteredQueryable;
    }

    private Expression FilterExpressionBuilder(List<BrowserDatasourceFilterItemParameter> items, ParameterExpression parameter)
    {
      Expression result = Expression.Empty();
      for (int i = 0; i < items.Count; i++)
      {
        BrowserDatasourceFilterItemParameter item = items[i];
        if (item is BrowserDatasourceFilterFieldParameter)
        {
          result = this.FilterExpressionFieldBuilder(item as BrowserDatasourceFilterFieldParameter, parameter);
        }

        if (item is BrowserDatasourceFilterListParameter && (item as BrowserDatasourceFilterListParameter).Items != null && (item as BrowserDatasourceFilterListParameter).Items.Count > 0)
        {
          result = this.FilterExpressionBuilder((item as BrowserDatasourceFilterListParameter).Items, parameter);
        }

        if (item is BrowserDatasourceFilterConditionParameter)
        {
          i++;
          BrowserDatasourceFilterItemParameter nextItem = items[i];
          Expression nextItemExpression = Expression.Empty();
          if (nextItem is BrowserDatasourceFilterFieldParameter)
          {
            nextItemExpression = this.FilterExpressionFieldBuilder(nextItem as BrowserDatasourceFilterFieldParameter, parameter);
          }

          if (nextItem is BrowserDatasourceFilterListParameter && (nextItem as BrowserDatasourceFilterListParameter).Items != null && (nextItem as BrowserDatasourceFilterListParameter).Items.Count > 0)
          {
            nextItemExpression = this.FilterExpressionBuilder((nextItem as BrowserDatasourceFilterListParameter).Items, parameter);
          }

          result = this.FilterExpressionConditionBuilder(item as BrowserDatasourceFilterConditionParameter, parameter, result, nextItemExpression);
        }
      }

      return result;
    }

    private Expression FilterExpressionFieldBuilder(BrowserDatasourceFilterFieldParameter item, ParameterExpression parameter)
    {
      Expression expression = Expression.Empty();

      Expression queryProperty = Expression.Empty();
      Expression value = Expression.Empty();

      Type fieldType = null;
      if (string.IsNullOrEmpty(item.Field))
      {
        return expression;
      }
      else
      {
        foreach (string fieldPart in item.Field.Split('.'))
        {
          PropertyInfo propertyInfo = (fieldType == null ? typeof(TEntity) : fieldType).GetProperty(fieldPart);
          if (propertyInfo == null) { return expression; }
          fieldType = propertyInfo.PropertyType;
        }
      }

      BrowserDatasourceFilterFieldParameter fieldItem = item as BrowserDatasourceFilterFieldParameter;
      MemberExpression property = this.GetPropertyOrField(parameter, fieldItem.Field);

      //MethodInfo objectToStringMethodInfo = typeof(object).GetMethod("ToString", System.Type.EmptyTypes);            
      //Expression propertyToString = Expression.Call(property, objectToStringMethodInfo);

      Expression propertyToString = property;

      MethodInfo stringToUpperMethodInfo = typeof(string).GetMethod("ToUpper", System.Type.EmptyTypes);

      if (fieldType == typeof(string))
      {
        queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
        value = Expression.Constant(fieldItem.Value.ToUpper());
      }
      else if (fieldType == typeof(int))
      {
        int intValue;
        bool canParse = int.TryParse(fieldItem.Value, out intValue);
        if (canParse)
        {
          queryProperty = property;
          value = Expression.Constant(intValue);
        }
        else
        {
          queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
          value = Expression.Constant(fieldItem.Value.ToUpper());
        }
      }
      else if (fieldType == typeof(decimal))
      {
        decimal decimalValue;
        bool canParse = decimal.TryParse(fieldItem.Value, out decimalValue);
        if (canParse)
        {
          queryProperty = property;
          value = Expression.Constant(decimalValue);
        }
        else
        {
          queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
          value = Expression.Constant(fieldItem.Value.ToUpper());
        }
      }
      else if (fieldType == typeof(DateTime))
      {
        //DateTime dateTimeValue = Converter.ToDateTime(fieldItem.Value);
        DateTime? dateTimeValue = Converter.ToDateTime(fieldItem.Value);
        if (dateTimeValue != null)
        {
          queryProperty = property;
          value = Expression.Constant(dateTimeValue.Value);
        }
        else
        {
          queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
          value = Expression.Constant(fieldItem.Value.ToUpper());
        }
      }
      else if (fieldType == typeof(bool))
      {
        bool? booleanValue = this.ConvertToBoolean(fieldItem.Value);
        if (booleanValue.HasValue)
        {
          queryProperty = property;
          value = Expression.Constant(booleanValue.Value);
        }
        else
        {
          queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
          value = Expression.Constant(fieldItem.Value.ToUpper());
        }
      }
      else
      {
        queryProperty = Expression.Call(propertyToString, stringToUpperMethodInfo);
        value = Expression.Constant(fieldItem.Value.ToUpper());
      }

      MethodInfo methodInfo;
      switch (fieldItem.Operation)
      {
        case "=":
          expression = Expression.Equal(queryProperty, value);
          break;
        case "<>":
          expression = Expression.NotEqual(queryProperty, value);
          break;
        case ">":
          expression = Expression.GreaterThan(queryProperty, value);
          break;
        case ">=":
          expression = Expression.GreaterThanOrEqual(queryProperty, value);
          break;
        case "<":
          expression = Expression.LessThan(queryProperty, value);
          break;
        case "<=":
          expression = Expression.LessThanOrEqual(queryProperty, value);
          break;
        case "startswith":
          methodInfo = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
          expression = Expression.Call(queryProperty, methodInfo, value);
          break;
        case "endswith":
          methodInfo = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
          expression = Expression.Call(queryProperty, methodInfo, value);
          break;
        case "contains":
          methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
          expression = Expression.Call(queryProperty, methodInfo, value);
          break;
        case "notcontains":
          methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
          expression = Expression.Not(Expression.Call(queryProperty, methodInfo, value));
          break;
      }

      return expression;
    }

    private Expression FilterExpressionConditionBuilder(BrowserDatasourceFilterConditionParameter item, ParameterExpression parameter, Expression left, Expression right)
    {
      Expression expression = Expression.Empty();
      if (item is BrowserDatasourceFilterConditionParameter)
      {
        BrowserDatasourceFilterConditionParameter conditionItem = item as BrowserDatasourceFilterConditionParameter;
        switch (conditionItem.Condition.ToUpper())
        {
          case "AND":
            expression = Expression.AndAlso(left, right);
            break;
          case "OR":
            expression = Expression.OrElse(left, right);
            break;
        }
      }

      return expression;
    }

    private bool? ConvertToBoolean(string value)
    {
      if (value.ToUpper() == "TRUE" || value == "1")
      {
        return true;
      }
      else if (value.ToUpper() == "FALSE" || value == "0")
      {
        return false;
      }
      return null;
    }






  }
}
