using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IIREventRepository
{
    Task AddAsync(IREvent irEvent, CancellationToken ct);
    Task UpdateAsync(IREvent irEvent, CancellationToken ct);
}
