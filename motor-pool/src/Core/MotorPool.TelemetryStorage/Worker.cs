using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MotorPool.TelemetryStorage.Configuration;
using MotorPool.TelemetryStorage.Messages;

namespace MotorPool.TelemetryStorage;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly BackingStorage _backingStorage;

    public Worker(IOptions<ApacheKafkaConfiguration> options, ILogger<Worker> logger, BackingStorage backingStorage)
    {
        _logger = logger;
        _backingStorage = backingStorage;

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
            await _backingStorage.AppendTelemetryAsync(telemetry, stoppingToken);
            _logger.LogInformation("Telemetry uploaded to blob: {Telemetry}", consumeResult.Message.Value);
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}