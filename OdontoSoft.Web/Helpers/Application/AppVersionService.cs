using System.Reflection;

namespace OdontoSoft.Web.Helpers.Application
{
  public class AppVersionService : IAppVersionService
  {
    public string Version =>
        Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
  }
}
