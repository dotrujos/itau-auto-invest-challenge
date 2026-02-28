using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IGraphicalAccountRepository
{
    /// <summary>
    /// Adiciona uma nova conta grafica e gera o numero da conta (FLH-XXXXXX) baseado no ID gerado.
    /// </summary>
    Task<GraphicalAccount> AddAndGenerateNumberAsync(GraphicalAccount account, CancellationToken ct);
    
    /// <summary>
    /// Buscar uma conta grafica por um ‘id’ do cliente.
    /// </summary>
    Task<GraphicalAccount?> GetByClientIdAsync(long clientId, CancellationToken ct);
}
