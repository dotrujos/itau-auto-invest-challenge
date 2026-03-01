namespace Itau.AutoInvest.Application.Abstractions;

public interface IKafkaProducer
{
    /// <summary>
    /// Publica uma mensagem em um tópico do Kafka de forma assíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto da mensagem.</typeparam>
    /// <param name="topic">Nome do tópico.</param>
    /// <param name="key">Chave da mensagem.</param>
    /// <param name="message">Corpo da mensagem (objeto).</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task PublishAsync<T>(string topic, string key, T message, CancellationToken ct);
}
