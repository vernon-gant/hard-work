using MotorPool.TelemetryStorage;
using MotorPool.TelemetryStorage.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<BlobStorageConfiguration>(builder.Configuration.GetSection("BlobStorageConfiguration"));
builder.Services.Configure<ApacheKafkaConfiguration>(builder.Configuration.GetSection("ApacheKafkaConfiguration"));
builder.Services.AddSingleton<BackingStorage, AzureBlobStorage>();

var host = builder.Build();
host.Run();