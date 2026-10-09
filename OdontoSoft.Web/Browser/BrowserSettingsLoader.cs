using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace OdontoSoft.Web.Browser
{
    public class BrowserSettingsLoader<TEntity> where TEntity : BaseEntity, IObjectState
    {


        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly String _configFileName;

        public BrowserSettingsLoader(IHttpContextAccessor accessor, LinkGenerator generator, String configFileName)
        {
            _accessor = accessor;
            _generator = generator;
            _configFileName = configFileName;
        }

        public BrowserSettings GetSettings()
        {
            string entityName = typeof(TEntity).Name;

            XmlTextReader xmlReader = new XmlTextReader(_configFileName);

            BrowserSettings browserSettings = new BrowserSettings(entityName, _accessor, _generator);

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "Setting")
                        {
                            string name = xmlReader.GetAttribute("Name");
                            string value = xmlReader.GetAttribute("Value");

                            PropertyInfo propertyInfo = browserSettings.GetType().GetProperty(name);
                            propertyInfo.SetValue(browserSettings, Convert.ChangeType(value, propertyInfo.PropertyType), null);

                        }
                        if (xmlReader.Name == "BrowserMainAction")
                        {
                            string name = xmlReader.GetAttribute("Name");
                            string action = xmlReader.GetAttribute("Action");
                            string controller = xmlReader.GetAttribute("Controller");
                            string permissionCode = xmlReader.GetAttribute("PermissionCode");
                            string enabled = xmlReader.GetAttribute("Enabled") ?? "";
                            string caption = xmlReader.GetAttribute("Caption") ?? "";


                            PropertyInfo propertyInfo = browserSettings.GetType().GetProperty(name);
                            if (propertyInfo != null && propertyInfo.PropertyType.Name == typeof(BrowserMainAction).Name)
                            {
                                BrowserMainAction propertyValue = propertyInfo.GetValue(browserSettings) as BrowserMainAction;
                                if (!string.IsNullOrEmpty(action))
                                {
                                    PropertyInfo actionPropertyInfo = propertyValue.GetType().GetProperty("Action");
                                    actionPropertyInfo.SetValue(propertyValue, Convert.ChangeType(action, actionPropertyInfo.PropertyType), null);
                                }

                                if (!string.IsNullOrEmpty(controller))
                                {
                                    PropertyInfo controllerPropertyInfo = propertyValue.GetType().GetProperty("Controller");
                                    controllerPropertyInfo.SetValue(propertyValue, Convert.ChangeType(controller, controllerPropertyInfo.PropertyType), null);
                                }

                                if (!string.IsNullOrEmpty(permissionCode))
                                {
                                    PropertyInfo controllerPropertyInfo = propertyValue.GetType().GetProperty("PermissionCode");
                                    controllerPropertyInfo.SetValue(propertyValue, Convert.ChangeType(permissionCode, controllerPropertyInfo.PropertyType), null);
                                }

                                if (!string.IsNullOrEmpty(enabled))
                                {
                                    PropertyInfo controllerPropertyInfo = propertyValue.GetType().GetProperty("Enabled");
                                    controllerPropertyInfo.SetValue(propertyValue, Convert.ChangeType(enabled.ToLower() == "true", controllerPropertyInfo.PropertyType), null);
                                }


                                if (!string.IsNullOrEmpty(caption))
                                {
                                    PropertyInfo controllerPropertyInfo = propertyValue.GetType().GetProperty("Caption");
                                    controllerPropertyInfo.SetValue(propertyValue, Convert.ChangeType(caption, controllerPropertyInfo.PropertyType), null);
                                }
                            }
                        }
                        break;
                }
            }

            xmlReader.Close();

            return browserSettings;
        }


    }
}
