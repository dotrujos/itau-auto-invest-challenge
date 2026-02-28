using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
