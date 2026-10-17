namespace OdontoSoft.Web.Helpers.Menu
{
  public class MenuCategory
  {
    public MenuCategory(String title, string icon, string categoryCode, bool permission)
    {
      this.Title = title;
      this.Icon = icon;
      this.CategoryCode = categoryCode;
      this.Permission = permission;
    }

    public string Title { get; set; }
    public string Icon { get; set; }
    public string CategoryCode { get; set; }
    public List<MenuItem> Items { get; set; }
    public List<MenuCategory> Submenus { get; set; }
    public bool Permission { get; set; }

  }
}
