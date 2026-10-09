using MedveCore.Common.Helpers;
using Mauro.RepositoryPattern.Infrastructure;
using System.Xml;

namespace OdontoSoft.Web.Browser
{
    public class BrowserActionsGenerator<TEntity> where TEntity : class, IObjectState
    {
        private readonly String _configFileName;

        public BrowserActionsGenerator(String configFileName)
        {
            _configFileName = configFileName;
        }
        public List<BrowserAction> GetActions()
        {
            XmlTextReader xmlReader = new XmlTextReader(_configFileName);

            var actions = new List<BrowserAction>();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "Action")
                        {
                            string text = xmlReader.GetAttribute("Text");
                            string action = xmlReader.GetAttribute("Action");
                            string controller = xmlReader.GetAttribute("Controller");

                            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
                            {
                                string iconCssClass = xmlReader.GetAttribute("IconCssClass");
                                string target = xmlReader.GetAttribute("Target");
                                string isAjaxRaw = xmlReader.GetAttribute("IsAjax");
                                bool isAjax = string.IsNullOrEmpty(isAjaxRaw) ? false : Converter.ToBoolean(isAjaxRaw.ToLower());
                                string confirmTitle = xmlReader.GetAttribute("ConfirmTitle");
                                confirmTitle = string.IsNullOrEmpty(confirmTitle) ? "Confirmación" : confirmTitle;
                                string confirmMessage = xmlReader.GetAttribute("ConfirmMessage");
                                confirmMessage = string.IsNullOrEmpty(confirmMessage) ? "¿Está seguro que desea realizar esta acción?" : confirmMessage;
                                string successTitle = xmlReader.GetAttribute("SuccessTitle");
                                successTitle = string.IsNullOrEmpty(successTitle) ? "Éxito" : successTitle;
                                string successMessage = xmlReader.GetAttribute("SuccessMessage");
                                successMessage = string.IsNullOrEmpty(successMessage) ? "La operación se ha realizado correctamente" : successMessage;

                                actions.Add(new BrowserAction
                                {
                                    Text = text,
                                    Action = action,
                                    Controller = controller,
                                    IconCssClass = iconCssClass,
                                    Target = target,
                                    IsAjax = isAjax,
                                    ConfirmTitle = confirmTitle,
                                    ConfirmMessage = confirmMessage,
                                    SuccessTitle = successTitle,
                                    SuccessMessage = successMessage
                                });
                            }
                        }
                        break;
                }
            }

            return actions;
        }
    }
}