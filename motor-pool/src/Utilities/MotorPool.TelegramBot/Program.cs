using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotorPool.Auth;
using MotorPool.Persistence;
using MotorPool.Repository;
using MotorPool.Services.Geo;
using MotorPool.Services.Geo.GraphHopper;
using MotorPool.Services.Reporting;
using MotorPool.TelegramBot;
using Serilog;
using Telegram.Bot;

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();

hostBuilder.Logging.ClearProviders();
hostBuilder.Logging.AddSerilog();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{hostBuilder.Environment.EnvironmentName}.json", optional: true)
    .Build();
hostBuilder.Services.AddSerilog(loggerConfiguration => loggerConfiguration.ReadFrom.Configuration(configuration));

string botToken = hostBuilder.Configuration.GetValue<string>("Telegram:BotToken") ?? throw new InvalidOperationException();
hostBuilder.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));
hostBuilder.Services.AddScoped<UpdateHandler, DefaultUpdateHandler>();
hostBuilder.Services.AddHostedService<BotHostedService>();
hostBuilder.Services.AddScoped<MessageResolver, DefaultMessageResolver>();
hostBuilder.Services.AddScoped<ActionFactory, DefaultActionFactory>();
hostBuilder.Services.AddSingleton<UserManager, DefaultUserManager>();
JWTConfiguration jwtConfiguration = new ();
hostBuilder.Configuration.GetSection("JWTConfig").Bind(jwtConfiguration);
hostBuilder.Services.AddSingleton(jwtConfiguration);
GraphHopperConfiguration graphHopperConfiguration = new ();
hostBuilder.Configuration.GetSection("GraphHopper").Bind(graphHopperConfiguration);
hostBuilder.Services.AddSingleton(graphHopperConfiguration);
string connectionString = hostBuilder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
hostBuilder.Services.AddPersistenceServices(connectionString);
hostBuilder.Services.AddRepository();
hostBuilder.Services.AddReporting();
hostBuilder.Services.AddGeoServices();
hostBuilder.Services.AddAppIdentity(connectionString);
hostBuilder.Services.AddScoped<PrintEnterEmail>();
hostBuilder.Services.AddScoped<ReadManagerEmail>();
hostBuilder.Services.AddScoped<ReadManagerPassword>();
hostBuilder.Services.AddScoped<WelcomeCommand>();
hostBuilder.Services.AddScoped<MenuCommand>();
hostBuilder.Services.AddScoped<LoginCommand>();
hostBuilder.Services.AddScoped<LogoutCommand>();
hostBuilder.Services.AddScoped<ReportCommand>();
hostBuilder.Services.AddScoped<UnknownCommand>();
hostBuilder.Services.AddScoped<Finished>();
hostBuilder.Services.AddScoped<PrintReportPeriod>();
hostBuilder.Services.AddScoped<ReadReportPeriod>();
hostBuilder.Services.AddScoped<ReadStartEndDates>();
hostBuilder.Services.AddScoped<ReadReportType>();
hostBuilder.Services.AddScoped<ReadVehicleId>();

hostBuilder.Build().Run();