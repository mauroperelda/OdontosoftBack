namespace OdontoSoft.Web.Browser
{

  public class BrowserModel
  {
    public Type EntityType { get; set; }
    public string Code { get; set; }
    public List<BrowserColumn> Columns { get; set; }
    public List<BrowserAction> Actions { get; set; }
    public IQueryable<dynamic> Datasource { get; set; }
    public int DatasourceTotalCount { get; set; }
    public BrowserSettings Settings { get; set; }
  }

}
