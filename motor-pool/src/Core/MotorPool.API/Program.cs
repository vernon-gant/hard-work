using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MotorPool.API.Cache;
using MotorPool.API.EndpointFilters;
using MotorPool.API.Endpoints;
using MotorPool.API.Producers;
using MotorPool.Auth;
using MotorPool.Auth.Middleware.API;
using MotorPool.Auth.Services;
using MotorPool.Persistence;
using MotorPool.Repository;
using MotorPool.Services.Drivers;
using MotorPool.Services.Enterprise;
using MotorPool.Services.Geo;
using MotorPool.Services.Geo.GraphHopper;
using MotorPool.Services.Reporting;
using MotorPool.Services.VehicleBrand;
using MotorPool.Services.Vehicles;
using MotorPool.Utils;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

JWTConfiguration jwtConfiguration = new();
builder.Configuration.GetSection("JWTConfig").Bind(jwtConfiguration);
builder.Services.AddSingleton(jwtConfiguration);

GraphHopperConfiguration graphHopperConfiguration = new();
builder.Configuration.GetSection("GraphHopper").Bind(graphHopperConfiguration);
builder.Services.AddSingleton(graphHopperConfiguration);

builder.Services.AddSingleton<TelemetryProducer>();
builder.Services.AddScoped<AuthService, DefaultAuthService>();
builder.Services.AddPersistenceServices(connectionString);
builder.Services.AddVehicleServices();
builder.Services.AddVehicleBrandServices();
builder.Services.AddEnterpriseServices();
builder.Services.AddDriverServices();
builder.Services.AddGeoServices();
builder.Services.AddReporting();
builder.Services.AddRepository();

builder.Services.AddAppIdentity(connectionString);
builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
                                                {
                                                    ValidateIssuerSigningKey = true,
                                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key)),
                                                    ValidIssuer = jwtConfiguration.Issuer,
                                                    ValidAudience = jwtConfiguration.Audience,
                                                    ValidateIssuer = false,
                                                    ValidateAudience = false
                                                };
        });
builder.Services.AddAppAuthorization();


builder.Services.AddSerilog((provider, config) => config.ReadFrom.Configuration(builder.Configuration)
                                                        .ReadFrom.Services(provider));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    OpenApiSecurityScheme jwtSecurityScheme = new()
                                              {
                                                  Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                                                  Name = "JWT Authentication",
                                                  BearerFormat = "JWT",
                                                  In = ParameterLocation.Header,
                                                  Type = SecuritySchemeType.Http,
                                                  Scheme = JwtBearerDefaults.AuthenticationScheme,

                                                  Reference = new OpenApiReference
                                                              {
                                                                  Id = JwtBearerDefaults.AuthenticationScheme,
                                                                  Type = ReferenceType.SecurityScheme
                                                              }
                                              };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                   {
                                       { jwtSecurityScheme, Array.Empty<string>() }
                                   });
});
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("IndividualAccess", policyBuilder => { policyBuilder.AddPolicy<AllowAuthorizationCachePolicy>().Expire(TimeSpan.FromMinutes(10)).SetVaryByHeader("Authorization"); }, excludeDefaultPolicy: true);

    options.AddPolicy("SharedAccess", policyBuilder => { policyBuilder.AddPolicy<AllowAuthorizationCachePolicy>().Expire(TimeSpan.FromMinutes(10)); }, excludeDefaultPolicy: true);
});

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCustomExceptionAPIMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseStatusCodePages();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseUnauthorizedOnNotManagerAccess();
app.UseAuthorization();
app.UseOutputCache();

app.MapVehicleBrandEndpoints();
app.MapAuthEndpoints();

RouteGroupBuilder managerResourcesGroupBuilder = app.MapGroup("/").RequireAuthorization().AddEndpointFilter<ManagerExistsFilter>();

managerResourcesGroupBuilder.MapVehicleEndpoints();
managerResourcesGroupBuilder.MapDriverEndpoints();
managerResourcesGroupBuilder.MapEnterpriseEndpoints();
managerResourcesGroupBuilder.MapReportEndpoints();
managerResourcesGroupBuilder.MapTelemetryEndpoints();

await app.SetupDatabaseAsync();
await app.SetupAuthDatabaseAsync();

app.Run();

public partial class Program
{
}