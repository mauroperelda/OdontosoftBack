using OdontoSoft.Web.Helpers.Application;
using OdontoSoft.Web.Helpers.Menu;
using Microsoft.AspNetCore.Mvc;

namespace OdontoSoft.Web.Helpers.Application
{
    public class CurrentApplication : ICurrentApplication
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CurrentApplication(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private HttpContext GetContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        public MenuItem? GetMenu(int id)
        {
            //var userAuthenticated = GetContext().RequestServices.GetRequiredService<IUserAuthenticated>();
            var applicationMenu = GetContext().RequestServices.GetRequiredService<ApplicationMenu>();

            var lista = this.GetMenuCategories(applicationMenu);
            foreach (var cat in lista)
            {
                if (cat.Items != null)
                {
                    foreach (var item in cat.Items)
                    {
                        if (item.Id == id)
                            return item;
                    }
                }
            }
            return null;
        }

        public LeftSideMenu GetLeftSideMenu()
        {
            LeftSideMenu leftSideMenu = new LeftSideMenu();

            var applicationMenu = GetContext().RequestServices.GetRequiredService<ApplicationMenu>();

            var menus = applicationMenu.GetModulos();
            leftSideMenu.MenuCategories = GetMenuCategories(applicationMenu);
            leftSideMenu.Modulo = menus == null ? new Modulo() : menus.FirstOrDefault(x => x.Id == applicationMenu.CodigoModulo());

            return leftSideMenu;
        }


        private List<MenuCategory>? GetMenuCategories(ApplicationMenu applicationMenu)
        {
            if (applicationMenu.CodigoModulo() == null)
                return null;

            return applicationMenu.GetModulos().FirstOrDefault(x => x.Id == applicationMenu.CodigoModulo()).Menu;
        }
    }
}
