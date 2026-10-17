using OdontoSoft.Web.Helpers.Menu;

namespace OdontoSoft.Web.Helpers.Application
{
  public interface ICurrentApplication
  {
    MenuItem? GetMenu(int id);
    LeftSideMenu GetLeftSideMenu();
    //LeftSideMenu GetLeftSideMenuClientes();
  }
}
