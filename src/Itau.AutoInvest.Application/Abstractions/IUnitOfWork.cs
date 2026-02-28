namespace Itau.AutoInvest.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
