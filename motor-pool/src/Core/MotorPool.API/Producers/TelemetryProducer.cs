using System.Text.Json;
using Confluent.Kafka;
using MotorPool.API.Messages;

namespace MotorPool.API.Producers;

public class TelemetryProducer
{
    private readonly string _telemetryTopic;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<TelemetryProducer> _logger;

    public TelemetryProducer(IConfiguration configuration, ILogger<TelemetryProducer> logger)
    {
        _logger = logger;
        var producerConfig = new ProducerConfig
                             {
                                 BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers"),
                                 Acks = Acks.None,
                             };
        _telemetryTopic = configuration.GetValue<string>("Kafka:TelemetryTopic") ?? throw new ArgumentNullException(nameof(_telemetryTopic));
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task ProduceTelemetryAsync(CANTelemetry telemetry)
    {
        string telemetryJson = JsonSerializer.Serialize(telemetry);
        await _producer.ProduceAsync(_telemetryTopic, new Message<string, string> { Key = telemetry.VehicleId.ToString(), Value = telemetryJson });
        _logger.LogInformation("Telemetry produced: {Telemetry}", telemetryJson);
    }
}