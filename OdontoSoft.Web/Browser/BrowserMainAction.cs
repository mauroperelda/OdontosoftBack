using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace OdontoSoft.Web.Browser
{
    public class BrowserMainAction
    {
        public string Caption { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string PermissionCode { get; set; }
        public bool Enabled { get; set; }

        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;

        public BrowserMainAction(IHttpContextAccessor accessor, LinkGenerator generator)
        {
            _accessor = accessor;
            _generator = generator;
        }

        public BrowserMainAction(string action, string controller, IHttpContextAccessor accessor, LinkGenerator generator)
        {
            this.Action = action;
            this.Controller = controller;

            _accessor = accessor;
            _generator = generator;
        }

        public string GetUrl()
        {
            var callbackLink = _accessor.HttpContext.Request.PathBase + _generator.GetPathByAction(this.Action, this.Controller);
            return callbackLink;
        }

        public string GetUrl(object routeValues)
        {
            var callbackLink = _accessor.HttpContext.Request.PathBase + _generator.GetPathByAction(this.Action, this.Controller, routeValues);
            return callbackLink;
        }
    }
}