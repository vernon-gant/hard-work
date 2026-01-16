using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using MotorPool.Auth;
using MotorPool.Persistence;
using MotorPool.Repository;
using MotorPool.Services.Drivers;
using MotorPool.Services.Enterprise;
using MotorPool.Services.Geo;
using MotorPool.Services.Geo.GraphHopper;
using MotorPool.Services.Manager;
using MotorPool.Services.Reporting;
using MotorPool.Services.VehicleBrand;
using MotorPool.Services.Vehicles;
using MotorPool.UI.PageFilters.AccessibilityFilters;
using MotorPool.UI.PageFilters.ExistenceFilters;
using MotorPool.Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

GraphHopperConfiguration graphHopperConfiguration = new ();
builder.Configuration.GetSection("GraphHopper").Bind(graphHopperConfiguration);
builder.Services.AddSingleton(graphHopperConfiguration);

builder.Services.AddPersistenceServices(connectionString);
builder.Services.AddVehicleServices();
builder.Services.AddVehicleBrandServices();
builder.Services.AddEnterpriseServices();
builder.Services.AddDriverServices();
builder.Services.AddAppIdentity(connectionString);
builder.Services.AddManagerServices();
builder.Services.AddGeoServices();
builder.Services.AddReporting();
builder.Services.AddRepository();

builder.Services
       .AddAuthentication(options =>
       {
           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       })
       .AddCookie(options =>
       {
           options.LoginPath = "/Identity/Account/Login";
           options.AccessDeniedPath = "/Identity/Account/AccessDenied";
       });
builder.Services.AddAppAuthorization();

builder.Services.AddSession();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var mvcBuilder = builder.Services
                        .AddRazorPages(options =>
                        {
                            options.Conventions.AuthorizeFolder("/Brands");
                            options.Conventions.AuthorizeFolder("/Drivers");
                            options.Conventions.AuthorizeFolder("/Enterprises");
                            options.Conventions.AuthorizeFolder("/Vehicles");
                            options.Conventions.AuthorizeFolder("/Reports");

                            options.Conventions.AddFolderApplicationModelConvention("/Vehicles", model =>
                            {
                                model.Filters.Add<IsManagerAccessibleVehicleFilter>();
                                model.Filters.Add<VehicleExistsPageFilter>();
                            });

                            options.Conventions.AddFolderApplicationModelConvention("/Drivers", model =>
                            {
                                model.Filters.Add<DriverExistsPageFilter>();
                            });

                            options.Conventions.AddFolderApplicationModelConvention("/Enterprises", model =>
                            {
                                model.Filters.Add<EnterpriseExistsPageFilter>();
                            });
                        })
                        .AddSessionStateTempDataProvider();

if (builder.Environment.IsDevelopment()) mvcBuilder.AddRazorRuntimeCompilation();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/Status", "?statusCode={0}");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("de-de"),
    SupportedCultures = new List<CultureInfo> { new ("en-US") },
    SupportedUICultures = new List<CultureInfo> { new ("en-US") }
});

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCustomExceptionUIMiddleware();

app.MapRazorPages();

await app.SetupDatabaseAsync();

await app.SetupAuthDatabaseAsync();

app.Run();