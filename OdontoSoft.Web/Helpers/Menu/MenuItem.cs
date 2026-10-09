namespace OdontoSoft.Web.Helpers.Menu
{
    public class MenuItem
    {
        public MenuItem(int id, string title, string controller, string action, string icon, bool permission)
        {
            this.Id = id;
            this.Title = title;
            this.Action = action;
            this.Controller = controller;
            this.Icon = icon;
            this.Permission = permission;
        }

        public MenuItem(int id, string title, string controller, string action, string icon)
        {
            this.Id = id;
            this.Title = title;
            this.Action = action;
            this.Controller = controller;
            this.Icon = icon;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool Permission { get; set; }

    }
}