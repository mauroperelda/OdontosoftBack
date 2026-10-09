using System;
using System.Collections.Generic;
using System.Linq;

namespace OdontoSoft.Web.Browser
{
    public interface IBrowser
    {
        Type EntityType { get; }
        string ViewTitle { get; set; }
        BrowserModel GetBrowserModel();
        BrowserModel GetBrowserModel(BrowserDatasourceParameters parameters);
        bool IsAllowed();
        bool IsAllowedCreateNew();
        bool IsAllowedEdit();

    }
}
