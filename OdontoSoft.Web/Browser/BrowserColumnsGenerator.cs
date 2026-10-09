using Mauro.RepositoryPattern.Infrastructure;
using System.Reflection;
using System.Xml;

namespace OdontoSoft.Web.Browser
{
    internal class BrowserColumnsGenerator<TEntity> where TEntity : class, IObjectState
    {

        private readonly String _configFileName;

        public BrowserColumnsGenerator(String configFileName)
        {
            _configFileName = configFileName;
        }

        public List<BrowserColumn> GetColumns()
        {
            XmlTextReader xmlReader = new XmlTextReader(_configFileName);

            var columns = new List<BrowserColumn>();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "Column")
                        {
                            string name = xmlReader.GetAttribute("Name");
                            string title = xmlReader.GetAttribute("Title");
                            BrowserColumnDataType dataType = BrowserColumnDataType.Other;

                            PropertyInfo propertyInfo = null;
                            if (string.IsNullOrEmpty(name)) { continue; }
                            else
                            {
                                foreach (string namePart in name.Split('.'))
                                {
                                    propertyInfo = (propertyInfo == null ? typeof(TEntity) : propertyInfo.PropertyType).GetProperty(namePart);
                                }
                            }

                            if (propertyInfo != null && propertyInfo.PropertyType != null)
                            {
                                if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                                {
                                    dataType = BrowserColumnDataType.Integer;
                                }
                                if (propertyInfo.PropertyType == typeof(string))
                                {
                                    dataType = BrowserColumnDataType.String;
                                }
                                if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                                {
                                    dataType = BrowserColumnDataType.Decimal;
                                }
                                if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                                {
                                    dataType = BrowserColumnDataType.Boolean;
                                }
                                if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                                {
                                    dataType = BrowserColumnDataType.DateTime;
                                }
                            }

                            columns.Add(new BrowserColumn { Name = name, Title = title, DataType = dataType });
                        }
                        break;
                }
            }

            if (columns.Count == 0) throw new Exception("Columnas no definidas en archivo");
            return columns;
        }
    }
}
