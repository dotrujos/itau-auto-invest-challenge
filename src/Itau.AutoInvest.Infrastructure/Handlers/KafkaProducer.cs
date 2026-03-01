using System.Text.Json;
using Confluent.Kafka;
using Itau.AutoInvest.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Infrastructure.Handlers;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = "Itau.AutoInvest.Api",
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = json
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, ct);
            
            _logger.LogInformation("Kafka Publish Success - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}", 
                result.Topic, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Kafka Publish Error - Topic: {Topic}, Reason: {Reason}", topic, ex.Error.Reason);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing to Kafka - Topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
