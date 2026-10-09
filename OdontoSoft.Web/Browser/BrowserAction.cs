using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OdontoSoft.Web.Browser
{
    public class BrowserAction
    {
        public string Text { get; set; }
        public string IconCssClass { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public bool IsAjax { get; set; }
        public string ConfirmTitle { get; set; }
        public string ConfirmMessage { get; set; }
        public string SuccessTitle { get; set; }
        public string SuccessMessage { get; set; }
    }
}