using System.Text.Json;
using Confluent.Kafka;

namespace MotorPool.TelemetryService;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private readonly IConsumer<string, string> _kafkaConsumer = new ConsumerBuilder<string, string>(new ConsumerConfig
                                                                                                    {
                                                                                                        BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers"),
                                                                                                        AutoOffsetReset = AutoOffsetReset.Earliest
                                                                                                    }).Build();
    private readonly string _vehicleTelemetryTopic = configuration.GetValue<string>("Kafka:VehicleTelemetryTopic")!;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Subscribe(_vehicleTelemetryTopic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<string, string> consumeResult = _kafkaConsumer.Consume(cancellationToken);

                CANTelemetry canTelemetry = JsonSerializer.Deserialize<CANTelemetry>(consumeResult.Message.Value) ?? throw new InvalidOperationException();

                logger.LogInformation("Telemetry consumed: {Telemetry}", canTelemetry);

                if (consumeResult.IsPartitionEOF) continue;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error consuming message from Kafka");
            }
        }

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Close();
        _kafkaConsumer.Dispose();
        return Task.CompletedTask;
    }
}