using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OdontoSoft.Web.Browser
{
    public class BrowserDatasourceParameters
    {
        public string BrowserCode { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public BrowserDatasourceSortParameter Sort { get; set; }
        public BrowserDatasourceGroupParameter Group { get; set; }
        public BrowserDatasourceFilterParameter Filter { get; set; }
        public BrowserDatasourceSearchParameter Search { get; set; }
    }

    public class BrowserDatasourceSortParameter
    {
        public List<BrowserDatasourceSortFieldParameter> Fields { get; set; }
    }

    public class BrowserDatasourceGroupParameter
    {
        public string DataField { get; set; }
        public List<BrowserDatasourceGroupFieldParameter> Fields { get; set; }
    }

    public class BrowserDatasourceFilterParameter
    {
        public List<BrowserDatasourceFilterItemParameter> Items { get; set; }
    }

    public class BrowserDatasourceSearchParameter
    {
        public string Value { get; set; }
        public string Operation { get; set; }
        public string Expression { get; set; }
    }

    public class BrowserDatasourceSortFieldParameter
    {
        public string Field { get; set; }
        public bool Descending { get; set; }
    }

    public class BrowserDatasourceGroupFieldParameter
    {
        public string Field { get; set; }
        public bool IsExpanded { get; set; }
    }

    public abstract class BrowserDatasourceFilterItemParameter
    {
    }

    public class BrowserDatasourceFilterFieldParameter : BrowserDatasourceFilterItemParameter
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operation { get; set; }
    }

    public class BrowserDatasourceFilterConditionParameter : BrowserDatasourceFilterItemParameter
    {
        public string Condition { get; set; }
    }

    public class BrowserDatasourceFilterListParameter : BrowserDatasourceFilterItemParameter
    {
        public List<BrowserDatasourceFilterItemParameter> Items { get; set; }
    }
}