using OdontoSoft.Web.Helpers.Application;
//using ExtraCore.Data.Authentication;
//using ExtraCore.Data.Helpers;
//using ExtraCore.Data.Repository;
//using ExtraCore.Data.Repository.UOW;
//using OdontoSoft.Domain.Models;
using OdontoSoft.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Net;
using System.Reflection;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Web.IServices;
using OdontoSoft.Web.Services;
using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Web.Startup;

// Restauramos el comportamiento legacy de Npgsql pre-6.0: permite escribir DateTime
// con Kind=Utc/Unspecified/Local en columnas "timestamp without time zone" sin
// lanzar excepción. Debe setearse ANTES de cualquier llamada a Npgsql.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OdontosoftContext>(options =>
{
  // Convención de naming: "Paciente" → tabla "paciente", "CreatedUserId" → "created_user_id".
  options.UseSnakeCaseNamingConvention();
  options.UseNpgsql(connString, b => b.MigrationsAssembly("OdontoSoft.Data"));
});

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
      options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
          _ => "El dato es requerido");
    });

builder.Services.AddTransient<ICurrentApplication, CurrentApplication>();
builder.Services.AddTransient<ApplicationMenu, ApplicationMenu>();
builder.Services.AddTransient<IAuthenticatedUser, AuthenticatedUser>();
//builder.Services.AddTransient<ILoggedUser, AuthenticatedUser>();
builder.Services.AddHttpClient<IAccountService, AccountService>();


//services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
builder.Services.AddTransient<IAppVersionService, AppVersionService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
      options.SerializerSettings.ContractResolver = new DefaultContractResolver();
      options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<LogFilter>();




builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromHours(8);//We set Time here 
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

builder.Services.AddMvc();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
      option.Cookie.Name = "OdontoSoftCookie";
      option.LoginPath = "/Account/Login";
      option.ExpireTimeSpan = TimeSpan.FromHours(8);
      option.SlidingExpiration = true;
    });

RegisterServices.Register(builder.Services);
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}


//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "Logout",
    pattern: "logout",
    defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "GetBrowserDatasource",
    pattern: "browser/GetBrowserDatasource",
    defaults: new { controller = "browser", action = "GetBrowserDatasource" });

app.MapControllerRoute(
    name: "browser",
    pattern: "browser/{code}",
    defaults: new { controller = "browser", action = "index" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseSession();

app.Run();
