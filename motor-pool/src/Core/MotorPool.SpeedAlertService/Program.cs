using Microsoft.Extensions.Options;
using MotorPool.SpeedAlertService;
using MotorPool.SpeedAlertService.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();
builder.Services.Configure<AzureConfiguration>(builder.Configuration.GetSection("AzureConfiguration"));
builder.Services.Configure<ApacheKafkaConfiguration>(builder.Configuration.GetSection("ApacheKafkaConfiguration"));

builder.Services.AddHttpClient<NotificationClient, HTTPLogicAppNotificationClient>()
       .ConfigureHttpClient((provider, client) =>
        {
            IOptions<AzureConfiguration> configuration = provider.GetRequiredService<IOptions<AzureConfiguration>>();
            client.BaseAddress = new Uri(configuration.Value.LogicAppEndpoint);
            client.DefaultRequestHeaders.Add("Accept","application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

var host = builder.Build();
host.Run();