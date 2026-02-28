using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IClientRepository
{
    /// <summary>
    /// Adiciona um novo cliente e retorna a entidade com o ID gerado.
    /// </summary>
    Task<Client> AddAsync(Client client, CancellationToken ct);

    /// <summary>
    /// Busca um cliente pelo CPF (RN-002).
    /// </summary>
    Task<Client?> GetByCpfAsync(string cpf, CancellationToken ct);

    /// <summary>
    /// Atualiza os dados de um cliente existente.
    /// </summary>
    Task UpdateAsync(Client client, CancellationToken ct);
}
