using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace OdontoSoft.Web.Browser
{
  public class BrowserSettings
  {

    public String Icon { get; set; }
    public String Title { get; set; }
    public String BrowserTitle { get; set; }
    public String PermissionCode { get; set; }

    public BrowserMainAction CreateNew { get; set; }
    public BrowserMainAction EditForm { get; set; }
    public BrowserMainAction Delete { get; set; }

    //public Boolean CanAddNew { get; set; }
    //public Boolean CanEdit { get; set; }
    //public Boolean CanDelete { get; set; }


    //public BrowserSettings(IHttpContextAccessor accessor, LinkGenerator generator)
    //{
    //    //this.CanEdit = true;
    //    //this.CanDelete = true;
    //    //this.CanAddNew = true;
    //    this.CreateNew = new BrowserMainAction(accessor, generator);
    //    this.EditForm = new BrowserMainAction(accessor, generator);
    //    this.Delete = new BrowserMainAction(accessor, generator);
    //}

    public BrowserSettings(string entityName, IHttpContextAccessor accessor, LinkGenerator generator)
    {
      //this.CanEdit = true;
      //this.CanDelete = true;
      //this.CanAddNew = true;
      this.CreateNew = new BrowserMainAction("EditForm", entityName, accessor, generator);
      this.EditForm = new BrowserMainAction("EditForm", entityName, accessor, generator);
      this.Delete = new BrowserMainAction("DeleteAjax", entityName, accessor, generator);
    }
  }
}
