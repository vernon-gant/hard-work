using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MotorPool.SpeedAlertService.Configuration;
using MotorPool.SpeedAlertService.Messages;

namespace MotorPool.SpeedAlertService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly NotificationClient _notificationClient;

    public Worker(IOptions<ApacheKafkaConfiguration> options, ILogger<Worker> logger, NotificationClient notificationClient)
    {
        _logger = logger;
        _notificationClient = notificationClient;

        var consumerConfig = new ConsumerConfig
                             {
                                 BootstrapServers = options.Value.BootstrapServers,
                                 GroupId = options.Value.GroupId,
                                 AutoOffsetReset = AutoOffsetReset.Earliest
                             };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _consumer.Subscribe(options.Value.TelemetryTopic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, string> consumeResult = _consumer.Consume(stoppingToken);

            if (consumeResult == null) continue;

            CANTelemetry telemetry = JsonSerializer.Deserialize<CANTelemetry>(consumeResult.Message.Value)!;

            CANTelemetryPayload payload = telemetry.ToPayload;

            if (telemetry.Speed <= 99) continue;

            await _notificationClient.PushAlert(payload);

            _logger.LogWarning("[ALERT] Driver on vehicle {VehicleId} is too fast - {Speed} km/h", payload.VehicleId, payload.Speed);
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}