#r "D:\Softwares\Odontosoft\OdontoSoft.Entities\bin\Debug\net9.0\Mauro.Entities.dll"
#r "D:\Softwares\Odontosoft\OdontoSoft.Entities\bin\Debug\net9.0\Mauro.RepositoryPattern.dll"
#r "D:\Softwares\Odontosoft\OdontoSoft.Entities\bin\Debug\net9.0\OdontoSoft.Entities.dll"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

string baseDir = @"D:\Softwares\Odontosoft";

// ─── Cargar tipos ───────────────────────────────────────────────────────────
var list = new List<Type>();
foreach (Type type in Assembly.GetAssembly(typeof(OdontoSoft.Entities.Models.Paciente))
    .GetTypes()
    .Where(x => x.Namespace == "OdontoSoft.Entities.Models")
    .ToList())
{
    if (type.Name.StartsWith("<>")) continue;
    if (type.IsAbstract) continue;
    list.Add(type);
}

Console.WriteLine($"Tipos encontrados: {list.Count}");

// ─── Generar archivos ────────────────────────────────────────────────────────

SaveFile(baseDir + @"\OdontoSoft.Data\DAL\Generated\", "OdontosoftContext.cs", Generate(list));

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Data\Repository\Interfaces\";
    string fileName = "I" + type.Name + "Repository.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateIRepo(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Data\Repository\Generated\";
    string fileName = type.Name + "Repository.cs";
    SaveFile(folder, fileName, GenerateRepo(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Data\Repository\";
    string fileName = type.Name + "Repository.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateRepoCustom(type));
}

SaveFile(baseDir + @"\OdontoSoft.Data\Repository\UOW\Generated\", "IUnitOfWork.cs",  GenerateIUnitOfWork(list));
SaveFile(baseDir + @"\OdontoSoft.Data\Repository\UOW\Generated\", "UnitOfWork.cs",   GenerateUnitOfWork(list));

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Services\Interface\";
    string fileName = "I" + type.Name + "Services.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateServiceInterface(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Services\CustomConstructors\";
    string fileName = type.Name + "Services.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateService(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Services\Custom\";
    string fileName = type.Name + "Services.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateCustomService(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Web\Config\Browser\Index\";
    string fileName = type.Name + ".xml";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateBrowserXml(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Web\Views\" + type.Name;
    string fileName = "EditForm.cshtml";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateEditForm(type));
}

SaveFile(baseDir + @"\OdontoSoft.Web\Browser\Generated\", "BrowserCacheHelper.cs", GenerateBrowsers(list));

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Web\Controllers\Generated\";
    string fileName = type.Name + "Controller.cs";
    SaveFile(folder, fileName, GenerateGeneratedController(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Web\Controllers\Custom\";
    string fileName = type.Name + "Controller.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateCustomController(type));
}

foreach (var type in list)
{
    string folder   = baseDir + @"\OdontoSoft.Web\Controllers\CustomConstructors\";
    string fileName = type.Name + "Controller.cs";
    string fullPath = Path.Combine(folder, fileName);
    if (!File.Exists(fullPath))
        SaveFile(folder, fileName, GenerateCustomControllerConstructor(type));
}

SaveFile(baseDir + @"\OdontoSoft.Web\Startup\", "RegisterServices.cs", GenerateServiceInjection(list));

Console.WriteLine("Generacion completada!");

// ═══════════════════════════════════════════════════════════════════════════
// HELPERS
// ═══════════════════════════════════════════════════════════════════════════

void SaveFile(string folder, string fileName, string content)
{
    if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

    using var fs  = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
    using var str = new StreamWriter(fs);
    str.WriteLine(content);
    str.Flush();
    Console.WriteLine($"  -> {Path.Combine(folder, fileName)}");
}

// ─── Generate ───────────────────────────────────────────────────────────────
string Generate(List<Type> inputList)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using Microsoft.EntityFrameworkCore;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.DAL");
    sb.AppendLine("{");
    sb.AppendLine("    public partial class OdontosoftContext");
    sb.AppendLine("    {");
    foreach (var type in inputList)
        sb.AppendLine($"        public virtual DbSet<{type.Name}> {type.Name} {{ get; set; }}");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateBrowsers ───────────────────────────────────────────────────────
string GenerateBrowsers(List<Type> inputList)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using OdontoSoft.Web.Controllers;");
    sb.AppendLine("using Microsoft.Extensions.Caching.Memory;");
    sb.AppendLine("using OdontoSoft.Web.Browser;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Web.Browser");
    sb.AppendLine("{");
    sb.AppendLine("    public partial class BrowserCacheHelper");
    sb.AppendLine("    {");
    sb.AppendLine("        private readonly ILogger _logger;");
    sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
    sb.AppendLine("        private readonly IHttpContextAccessor _accessor;");
    sb.AppendLine("        private readonly LinkGenerator _generator;");
    sb.AppendLine("        private readonly IHostEnvironment _hostEnvironment;");
    sb.AppendLine("        private readonly IMemoryCache _memoryCache;");
    sb.AppendLine("        private readonly IAuthenticatedUser _authenticatedUser;");
    sb.AppendLine();
    sb.AppendLine("        public BrowserCacheHelper(ILogger logger, IUnitOfWork unitOfWork");
    sb.AppendLine("            , IHttpContextAccessor accessor, LinkGenerator generator");
    sb.AppendLine("            , IHostEnvironment hostEnvironment, IMemoryCache memoryCache");
    sb.AppendLine("            , IAuthenticatedUser authenticatedUser)");
    sb.AppendLine("        {");
    sb.AppendLine("            _logger = logger;");
    sb.AppendLine("            _unitOfWork = unitOfWork;");
    sb.AppendLine("            _accessor = accessor;");
    sb.AppendLine("            _generator = generator;");
    sb.AppendLine("            _hostEnvironment = hostEnvironment;");
    sb.AppendLine("            _memoryCache = memoryCache;");
    sb.AppendLine("            _authenticatedUser = authenticatedUser;");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine("        public void AddBrowsers()");
    sb.AppendLine("        {");
    foreach (var type in inputList)
        sb.AppendLine($"            _memoryCache.Set(\"BROWSER_{type.Name}\".ToUpper(), new Browser<{type.Name}>(_accessor, _generator, _hostEnvironment, _authenticatedUser));");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine("        public IBrowser GetBrowser(string code)");
    sb.AppendLine("        {");
    sb.AppendLine("            IBrowser b;");
    sb.AppendLine("            code = (\"BROWSER_\" + code).ToUpper();");
    sb.AppendLine("            if (!this._memoryCache.TryGetValue(code, out b))");
    sb.AppendLine("                AddBrowsers();");
    sb.AppendLine("            if (!this._memoryCache.TryGetValue(code, out b))");
    sb.AppendLine("                throw new Exception(\"BROWSER_\" + code.ToUpper() + \". Browser is not registered. Please regenerate Browser Registration file.\");");
    sb.AppendLine("            if (b == null)");
    sb.AppendLine("                throw new Exception(\"BROWSER_\" + code.ToUpper() + \". Browser is not registered. Please regenerate Browser Registration file.\");");
    sb.AppendLine("            return b;");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateIUnitOfWork ────────────────────────────────────────────────────
string GenerateIUnitOfWork(List<Type> inputList)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Repository.Interfaces;");
    sb.AppendLine("using Mauro.Entities.Base;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.Repository.UOW");
    sb.AppendLine("{");
    sb.AppendLine("    public partial interface IUnitOfWork");
    sb.AppendLine("    {");
    foreach (var type in inputList)
        sb.AppendLine($"        I{type.Name}Repository {type.Name}Repository {{ get; }}");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateUnitOfWork ─────────────────────────────────────────────────────
string GenerateUnitOfWork(List<Type> inputList)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Repository.Interfaces;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.Repository.UOW");
    sb.AppendLine("{");
    sb.AppendLine("    public partial class UnitOfWork");
    sb.AppendLine("    {");
    foreach (var type in inputList)
        sb.AppendLine($"        public I{type.Name}Repository {type.Name}Repository {{ get; private set; }}");
    sb.AppendLine();
    sb.AppendLine("        private void InstanceRepos()");
    sb.AppendLine("        {");
    foreach (var type in inputList)
        sb.AppendLine($"            {type.Name}Repository = new {type.Name}Repository(_context, _logger, _authenticatedUser);");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateServiceInjection ───────────────────────────────────────────────
string GenerateServiceInjection(List<Type> inputList)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Services;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Web.Startup");
    sb.AppendLine("{");
    sb.AppendLine("    public class RegisterServices");
    sb.AppendLine("    {");
    sb.AppendLine("        public static void Register(IServiceCollection services)");
    sb.AppendLine("        {");
    foreach (var type in inputList)
        sb.AppendLine($"            services.AddTransient<I{type.Name}Services, {type.Name}Services>();");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateIRepo ──────────────────────────────────────────────────────────
string GenerateIRepo(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.Repository.Interfaces");
    sb.AppendLine("{");
    sb.AppendLine($"    public interface I{type.Name}Repository : IGenericRepository<{type.Name}>");
    sb.AppendLine("    {");
    sb.AppendLine($"        /*Ejemplo de nuevos métodos - Estos métodos se implementan en {type.Name}Repository*/");
    sb.AppendLine($"        //{type.Name} GetByMiCampo(int miCampoId);");
    sb.AppendLine($"        //IQueryable<{type.Name}> GetByMiOtroCampo(int miOtroCampoId);");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateRepo ───────────────────────────────────────────────────────────
string GenerateRepo(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.DAL;");
    sb.AppendLine("using OdontoSoft.Data.Repository.Interfaces;");
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using Microsoft.EntityFrameworkCore;");
    sb.AppendLine("using Microsoft.Extensions.Logging;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.Repository");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Repository : GenericRepository<{type.Name}>, I{type.Name}Repository");
    sb.AppendLine("    {");
    sb.AppendLine($"        public {type.Name}Repository(OdontosoftContext context, ILogger logger, IAuthenticatedUser authenticatedUser) : base(context, logger, authenticatedUser)");
    sb.AppendLine("        {");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateRepoCustom ─────────────────────────────────────────────────────
string GenerateRepoCustom(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.DAL;");
    sb.AppendLine("using OdontoSoft.Data.Repository.Interfaces;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using Microsoft.EntityFrameworkCore;");
    sb.AppendLine("using Microsoft.Extensions.Logging;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Data.Repository");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Repository : GenericRepository<{type.Name}>, I{type.Name}Repository");
    sb.AppendLine("    {");
    sb.AppendLine($"        /*Ejemplo de reemplazo de métodos existentes*/");
    sb.AppendLine($"        //public override {type.Name} GetById(int id)");
    sb.AppendLine("        //{{");
    sb.AppendLine("        //    return All()");
    sb.AppendLine("        //        .Include(x => x.ProvinciaId)");
    sb.AppendLine("        //        .FirstOrDefault(x=>x.Id == id);");
    sb.AppendLine("        //}}");
    sb.AppendLine($"        //public override IQueryable<{type.Name}> AllForBrowser()");
    sb.AppendLine("        //{{");
    sb.AppendLine("        //    return base.AllForBrowser()");
    sb.AppendLine("        //        .Include(x => x.Provincia)");
    sb.AppendLine("        //        .OrderBy(x=>x.Descripcion);");
    sb.AppendLine("        //}}");
    sb.AppendLine($"        //public {type.Name} GetByMiCampo(int miCampoId)");
    sb.AppendLine("        //{{");
    sb.AppendLine("        //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);");
    sb.AppendLine("        //}}");
    sb.AppendLine($"        //public IQueryable<{type.Name}> GetByMiOtroCampo(int miOtroCampoId)");
    sb.AppendLine("        //{{");
    sb.AppendLine("        //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);");
    sb.AppendLine("        //}}");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateService ────────────────────────────────────────────────────────
string GenerateService(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using Microsoft.Extensions.Logging;");
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using Odontosoft.Services;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Services");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Services : BaseServices, I{type.Name}Services");
    sb.AppendLine("    {");
    sb.AppendLine($"        public {type.Name}Services(ILogger<{type.Name}Services> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)");
    sb.AppendLine("        {");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateCustomService ──────────────────────────────────────────────────
string GenerateCustomService(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using Microsoft.Extensions.Logging;");
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using Odontosoft.Services;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Services");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Services : BaseServices, I{type.Name}Services");
    sb.AppendLine("    {");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateServiceInterface ───────────────────────────────────────────────
string GenerateServiceInterface(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using Microsoft.Extensions.Logging;");
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using Odontosoft.Services;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Services");
    sb.AppendLine("{");
    sb.AppendLine($"    public interface I{type.Name}Services");
    sb.AppendLine("    {");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateBrowserXml ─────────────────────────────────────────────────────
string GenerateBrowserXml(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
    sb.AppendLine("<Browser>");
    sb.AppendLine("  <Settings>");
    sb.AppendLine($"    <Setting Name=\"Title\" Value=\"{type.Name}\"/>");
    sb.AppendLine($"    <Setting Name=\"BrowserTitle\" Value=\"Administración de {type.Name}\"/>");
    sb.AppendLine("    <Setting Name=\"Icon\" Value=\"uil-bookmark-full\"/>");
    sb.AppendLine("    <Setting Name=\"PermissionCode\" Value=\"\"/>");
    sb.AppendLine($"    <BrowserMainAction Name=\"CreateNew\" Enabled=\"true\" Action=\"EditForm\" Controller=\"{type.Name}\" PermissionCode=\"\" Caption=\"Agregar {type.Name}\"/>");
    sb.AppendLine($"    <BrowserMainAction Name=\"EditForm\" Enabled=\"true\" Action=\"EditForm\" Controller=\"{type.Name}\" PermissionCode=\"\"/>");
    sb.AppendLine($"    <BrowserMainAction Name=\"Delete\" Enabled=\"true\" Action=\"DeleteAjax\" Controller=\"{type.Name}\" PermissionCode=\"\"/>");
    sb.AppendLine("  </Settings>");
    sb.AppendLine("  <Columns>");
    foreach (var prop in type.GetProperties())
    {
        if (prop.Name != "Id" && prop.Name != "EntityName" && prop.Name != "GetString" && prop.Name != "ObjectState")
            sb.AppendLine($"    <Column Name=\"{prop.Name}\" Title=\"{prop.Name}\" />");
    }
    sb.AppendLine("  </Columns>");
    sb.AppendLine("  <Actions>");
    sb.AppendLine("    <!-- <Action Text=\"Reporte\" IconCssClass=\"fa fa-file-excel-o\" Action=\"DownloadReport\" Controller=\"SomeController\" /> -->");
    sb.AppendLine("  </Actions>");
    sb.AppendLine("</Browser>");
    return sb.ToString();
}

// ─── GenerateEditForm ───────────────────────────────────────────────────────
string GenerateEditForm(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine($"@model {type}");
    sb.AppendLine("@using OdontoSoft.Web.Helpers");
    sb.AppendLine();
    sb.AppendLine("@{");
    sb.AppendLine("    ViewBag.Title = \"Formulario\";");
    sb.AppendLine("}");
    sb.AppendLine();
    sb.AppendLine("<div class=\"row\">");
    sb.AppendLine("    <div class=\"col-12\">");
    sb.AppendLine("        <div class=\"page-title-box\">");
    sb.AppendLine($"            <h4 class=\"page-title\">Formulario de {type.Name}</h4>");
    sb.AppendLine("        </div>");
    sb.AppendLine("    </div>");
    sb.AppendLine("</div>");
    sb.AppendLine();
    sb.AppendLine("<div class=\"row\">");
    sb.AppendLine("    <div class=\"col-12\">");
    sb.AppendLine("        <div class=\"card\">");
    sb.AppendLine("            <div class=\"card-body\">");
    sb.AppendLine("                @using (Html.BeginForm())");
    sb.AppendLine("                {");
    sb.AppendLine("                    @Html.AntiForgeryToken()");
    sb.AppendLine("                    <div class=\"row mb-3\">");
    sb.AppendLine("                        <div class=\"col-md-12\">");
    sb.AppendLine("                            @Html.ValidationSummary(true)");
    sb.AppendLine("                            @Html.HiddenFor(model => model.Id)");
    sb.AppendLine("                        </div>");
    sb.AppendLine("                    </div>");

    foreach (var props in type.GetProperties())
    {
        if (AplicaProp(props) && !EsPropFK(props))
        {
            sb.AppendLine("                    <div class=\"row mb-3\">");
            sb.AppendLine("                        <div class=\"col-md-6\">");
            sb.AppendLine($"                            @Html.LabelFor(x => x.{props.Name})");
            sb.AppendLine($"                            {GetFieldHtml(props)}");
            sb.AppendLine($"                            @Html.ValidationMessageFor(model => model.{props.Name})");
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    </div>");
        }
    }

    sb.AppendLine("                    <div class=\"row mb-3\">");
    sb.AppendLine("                        <div class=\"col-md-6\">");
    sb.AppendLine("                            Deshabilitado<br />");
    sb.AppendLine("                            @Html.CheckBoxFor(x => x.Disabled, new { @class = \"form-check-input\"})");
    sb.AppendLine("                            @Html.ValidationMessageFor(model => model.Disabled)");
    sb.AppendLine("                        </div>");
    sb.AppendLine("                    </div>");
    sb.AppendLine("                    <div class=\"row mb-3\">");
    sb.AppendLine("                        <div class=\"col-md-12 button-list\">");
    sb.AppendLine("                            <input type=\"submit\" class=\"btn btn-primary\" value=\"Guardar\" />");
    sb.AppendLine("                            <a class=\"btn text-muted d-none d-sm-inline-block btn-link fw-semibold\" href=\"@Url.Action(\"Index\", \"Browser\", new { code = Model.GetType().Name })\"><i class=\"mdi mdi-arrow-left\"></i>Volver al listado</a>");
    sb.AppendLine("                        </div>");
    sb.AppendLine("                    </div>");
    sb.AppendLine("                }");
    sb.AppendLine("            </div>");
    sb.AppendLine("        </div>");
    sb.AppendLine("    </div>");
    sb.AppendLine("</div>");
    return sb.ToString();
}

// ─── GenerateGeneratedController ────────────────────────────────────────────
string GenerateGeneratedController(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using OdontoSoft.Web.Helpers.Authentication;");
    sb.AppendLine("using OdontoSoft.Web.Browser;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
    sb.AppendLine("using Microsoft.Extensions.Caching.Memory;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc.ModelBinding;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Web.Controllers");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Controller : AppBaseBrowserController");
    sb.AppendLine("    {");
    sb.AppendLine("        private int _entityId;");
    sb.AppendLine();
    sb.AppendLine("        public ActionResult EditForm(int id = 0)");
    sb.AppendLine("        {");
    sb.AppendLine("            if (!AllowAction(id)) return Unauthorized();");
    sb.AppendLine($"            {type.Name} entity;");
    sb.AppendLine("            if (id != 0)");
    sb.AppendLine($"                entity = _unitOfWork.{type.Name}Repository.GetById(id);");
    sb.AppendLine("            else");
    sb.AppendLine($"                entity = new {type.Name}();");
    sb.AppendLine("            PrepareEntityBeforeLoadForm(entity);");
    sb.AppendLine("            base.LoadViewBag(PrepareViewBag(entity));");
    sb.AppendLine("            return View(entity);");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine("        [HttpPost]");
    sb.AppendLine($"        public ActionResult EditForm({type.Name} entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)");
    sb.AppendLine("        {");
    sb.AppendLine("            if (!AllowAction(entity.Id)) return Unauthorized();");
    sb.AppendLine("            if (ModelState.IsValid)");
    sb.AppendLine("            {");
    sb.AppendLine("                try");
    sb.AppendLine("                {");
    sb.AppendLine($"                    {type.Name} currentModel = null;");
    sb.AppendLine("                    if (entity.Id != 0)");
    sb.AppendLine("                    {");
    sb.AppendLine($"                        currentModel = _unitOfWork.{type.Name}Repository.GetById(entity.Id);");
    sb.AppendLine("                        TryUpdateModelAsync(currentModel);");
    sb.AppendLine("                    }");
    sb.AppendLine("                    else");
    sb.AppendLine("                        currentModel = entity;");
    sb.AppendLine("                    PrepareEntityFromFormData(currentModel, formCollection);");
    sb.AppendLine("                    ActionsBeforeSave(currentModel, formCollection);");
    sb.AppendLine("                    if (entity.Id != 0)");
    sb.AppendLine($"                        _unitOfWork.{type.Name}Repository.Upsert(currentModel);");
    sb.AppendLine("                    else");
    sb.AppendLine($"                        _unitOfWork.{type.Name}Repository.Add(currentModel);");
    sb.AppendLine("                    _unitOfWork.Save();");
    sb.AppendLine("                    _entityId = currentModel.Id;");
    sb.AppendLine("                    ModelState.SetModelValue(\"Id\", new ValueProviderResult(currentModel.Id.ToString()));");
    sb.AppendLine($"                    entity = _unitOfWork.{type.Name}Repository.GetById(_entityId);");
    sb.AppendLine("                    ActionsAfterSave(currentModel, formCollection);");
    sb.AppendLine("                    return RedirectToBrowser(formCollection);");
    sb.AppendLine("                }");
    sb.AppendLine("                catch (Exception ex)");
    sb.AppendLine("                {");
    sb.AppendLine("                    if (ex.InnerException != null)");
    sb.AppendLine("                        if (ex.InnerException.InnerException != null)");
    sb.AppendLine("                            ModelState.AddModelError(string.Empty, ex.InnerException.InnerException.Message);");
    sb.AppendLine("                        else");
    sb.AppendLine("                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);");
    sb.AppendLine("                    else");
    sb.AppendLine("                        ModelState.AddModelError(string.Empty, ex.Message);");
    sb.AppendLine("                }");
    sb.AppendLine("            }");
    sb.AppendLine("            PrepareEntityFromFormData(entity, formCollection);");
    sb.AppendLine("            base.LoadViewBag(PrepareViewBag(entity));");
    sb.AppendLine("            return View(entity);");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine("        public JsonResult DeleteAjax(int id)");
    sb.AppendLine("        {");
    sb.AppendLine("            try");
    sb.AppendLine("            {");
    sb.AppendLine($"                _unitOfWork.{type.Name}Repository.Delete(id);");
    sb.AppendLine("                _unitOfWork.Save();");
    sb.AppendLine("                return JsonOk();");
    sb.AppendLine("            }");
    sb.AppendLine("            catch (Exception ex)");
    sb.AppendLine("            {");
    sb.AppendLine("                return JsonError(ex.Message);");
    sb.AppendLine("            }");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateCustomController ───────────────────────────────────────────────
string GenerateCustomController(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
    sb.AppendLine("using Microsoft.Extensions.Caching.Memory;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc.Rendering;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Web.Controllers");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Controller : AppBaseBrowserController");
    sb.AppendLine("    {");
    sb.AppendLine($"        private Dictionary<string, object> PrepareViewBag({type.Name} entity)");
    sb.AppendLine("        {");
    sb.AppendLine("            Dictionary<string, object> viewBagData = new Dictionary<string, object>();");

    foreach (var props in type.GetProperties())
    {
        if (IsDropDownListInt(props))
        {
            string realPropName = FirstCharacterToLower(props.Name.Replace("Id", "").Replace("Parent", ""));
            string listName = realPropName + "List";
            string repoName = GetPropertyTypeBasedOnIdField(type, props.Name);

            if (Nullable.GetUnderlyingType(props.PropertyType) != null)
                sb.AppendLine($"            SelectList {listName} = new SelectList(_unitOfWork.{repoName}Repository.GetAllEnabled(entity.{props.Name} ?? 0).ToList(), \"Id\", \"GetString\", entity.{props.Name} ?? 0);");
            else
                sb.AppendLine($"            SelectList {listName} = new SelectList(_unitOfWork.{repoName}Repository.GetAllEnabled(entity.{props.Name}).ToList(), \"Id\", \"GetString\", entity.{props.Name});");

            sb.AppendLine($"            viewBagData.Add(\"{realPropName}\", {listName});");
        }
    }

    sb.AppendLine("            return viewBagData;");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine($"        private void PrepareEntityFromFormData({type.Name} entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)");
    sb.AppendLine("        {");
    sb.AppendLine("            //Implement here any functionallity to update the entity from specific data in formCollection.");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine($"        private void PrepareEntityBeforeLoadForm({type.Name} entity)");
    sb.AppendLine("        {");
    sb.AppendLine("            //Implement here any functionallity to update the entity before EditForm shows.");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine($"        private void ActionsBeforeSave({type.Name} entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)");
    sb.AppendLine("        {");
    sb.AppendLine("            //Implement here any functionallity to execute any action before unitofworks.Save");
    sb.AppendLine("        }");
    sb.AppendLine();
    sb.AppendLine($"        private void ActionsAfterSave({type.Name} entity, Microsoft.AspNetCore.Http.IFormCollection formCollection)");
    sb.AppendLine("        {");
    sb.AppendLine("            //Implement here any functionallity to execute any action after unitofworks.Save");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ─── GenerateCustomControllerConstructor ────────────────────────────────────
string GenerateCustomControllerConstructor(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine("using OdontoSoft.Data.Repository.UOW;");
    sb.AppendLine("using OdontoSoft.Entities.Models;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
    sb.AppendLine("using Microsoft.Extensions.Caching.Memory;");
    sb.AppendLine("using Microsoft.AspNetCore.Mvc.Rendering;");
    sb.AppendLine("using OdontoSoft.Data.Authentication;");
    sb.AppendLine("using OdontoSoft.Web.Browser;");
    sb.AppendLine();
    sb.AppendLine("namespace OdontoSoft.Web.Controllers");
    sb.AppendLine("{");
    sb.AppendLine($"    public partial class {type.Name}Controller : AppBaseBrowserController");
    sb.AppendLine("    {");
    sb.AppendLine($"        private readonly ILogger<{type.Name}Controller> _logger;");
    sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
    sb.AppendLine("        private readonly IHttpContextAccessor _accessor;");
    sb.AppendLine("        private readonly LinkGenerator _generator;");
    sb.AppendLine("        private readonly IHostEnvironment _hostEnvironment;");
    sb.AppendLine("        private readonly IMemoryCache _memoryCache;");
    sb.AppendLine("        private readonly IAuthenticatedUser _authenticatedUser;");
    sb.AppendLine();
    sb.AppendLine($"        public {type.Name}Controller(ILogger<{type.Name}Controller> logger, IUnitOfWork unitOfWork,");
    sb.AppendLine("            IHttpContextAccessor accessor, LinkGenerator generator,");
    sb.AppendLine("            IHostEnvironment hostEnvironment, IMemoryCache memoryCache,");
    sb.AppendLine($"            IAuthenticatedUser authenticatedUser) : base(\"{type.Name}\")");
    sb.AppendLine("        {");
    sb.AppendLine("            _logger = logger;");
    sb.AppendLine("            _unitOfWork = unitOfWork;");
    sb.AppendLine("            _accessor = accessor;");
    sb.AppendLine("            _generator = generator;");
    sb.AppendLine("            _hostEnvironment = hostEnvironment;");
    sb.AppendLine("            _memoryCache = memoryCache;");
    sb.AppendLine("            _authenticatedUser = authenticatedUser;");
    sb.AppendLine("            _browserCacheHelper = new BrowserCacheHelper(_logger, _unitOfWork, _accessor, _generator, _hostEnvironment, _memoryCache, _authenticatedUser);");
    sb.AppendLine("        }");
    sb.AppendLine("    }");
    sb.AppendLine("}");
    return sb.ToString();
}

// ═══════════════════════════════════════════════════════════════════════════
// FUNCIONES AUXILIARES
// ═══════════════════════════════════════════════════════════════════════════

bool IsDropDownListInt(PropertyInfo prop)
{
    if ((prop.PropertyType.Name == "Int32" || prop.PropertyType.Name.Contains("Nullable"))
        && prop.Name.EndsWith("Id") && AplicaProp(prop))
        return true;
    return false;
}

bool AplicaProp(PropertyInfo prop)
{
    if (prop.PropertyType.Name.StartsWith("System.Collections.Generic.List")) return false;
    if (prop.Name == "Disabled")         return false;
    if (prop.Name == "ObjectState")      return false;
    if (prop.Name == "EntityName")       return false;
    if (prop.Name == "GetString")        return false;
    if (prop.Name == "CreatedUserId")    return false;
    if (prop.Name == "CreatedUserName")  return false;
    if (prop.Name == "CreatedDate")      return false;
    if (prop.Name == "UpdatedUserId")    return false;
    if (prop.Name == "UpdatedUserName")  return false;
    if (prop.Name == "UpdatedDate")      return false;
    if (prop.Name == "Id")               return false;
    if (prop.PropertyType.Name.EndsWith("Id")) return false;
    return true;
}

bool EsPropFK(PropertyInfo prop) => prop.Name.EndsWith("Id");

string FirstCharacterToLower(string str)
{
    if (string.IsNullOrEmpty(str) || char.IsLower(str, 0)) return str;
    return char.ToLowerInvariant(str[0]) + str.Substring(1);
}

string GetPropertyTypeBasedOnIdField(Type type, string idPropName)
{
    string propName = idPropName;
    if (propName.EndsWith("Id"))
        propName = propName.Remove(propName.Length - 2);

    foreach (var prop in type.GetProperties())
        if (prop.Name == propName)
            return prop.PropertyType.Name;

    throw new Exception($"No se encontró la propiedad: {propName} en {type.Name}");
}

string GetFieldHtml(PropertyInfo prop)
{
    if (prop.PropertyType.Name == "String")
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new {{ @class = \"form-control\" }})";

    if (prop.PropertyType.Name == "Decimal")
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.DecimalSettings(2) }} }})";

    if (prop.PropertyType == typeof(decimal?) && !prop.Name.EndsWith("Id"))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.NullableDecimalSettings(2) }} }})";

    if (prop.PropertyType.Name == "Int32" && !prop.Name.EndsWith("Id"))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.IntegerSettings() }} }})";

    if (prop.PropertyType == typeof(int?) && !prop.Name.EndsWith("Id"))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.NullableIntegerSettings() }} }})";

    if (prop.PropertyType.Name == "Int64" && !prop.Name.EndsWith("Id"))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.IntegerSettings() }} }})";

    if (prop.PropertyType == typeof(long?) && !prop.Name.EndsWith("Id"))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, new Dictionary<string, object> {{ {{ \"class\", \"form-control data-autonumeric-field\" }}, {{ \"data-autonumeric-settings\", AutoNumericHelper.NullableIntegerSettings() }} }})";

    if (prop.PropertyType.Name == "DateTime" || prop.PropertyType == typeof(DateTime?))
        return $"@Html.TextBoxFor(x => x.{prop.Name}, \"{{0:dd/MM/yyyy}}\", new {{ @class = \"form-control datepicker\" }})";

    if (prop.PropertyType.Name == "Boolean")
        return $"@Html.CheckBoxFor(x => x.{prop.Name}, new {{ @class = \"form-check-input\"}})";

    if (prop.PropertyType.BaseType != null)
        if (prop.PropertyType.BaseType.Name.StartsWith("AuditedEntity") || prop.PropertyType.BaseType.Name.StartsWith("Entity"))
            return $"@Html.DropDownList(\"{prop.Name}Id\", (SelectList)ViewBag.{prop.Name}, String.Empty, new Dictionary<string, object> {{ {{ \"class\", \"form-select\" }} }})";

    return $"@*TIPO NO DEFINIDO - {prop.PropertyType} -*@ @Html.TextBoxFor(x => x.{prop.Name}, new {{ @class = \"form-control\" }})";
}
