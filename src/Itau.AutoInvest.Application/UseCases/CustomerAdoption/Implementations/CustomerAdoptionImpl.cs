using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.DTOs;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Application.UseCases.CustomerAdoption.Implementations;

public class CustomerAdoptionImpl : CustomerAdoption
{
    private readonly IGraphicalAccountRepository _graphicalAccountRepository;
    private readonly IClientRepository _clientRepository;
    
    public CustomerAdoptionImpl(IGraphicalAccountRepository graphicalAccountRepository, IClientRepository clientRepository)
    {
        _graphicalAccountRepository = graphicalAccountRepository;
        _clientRepository = clientRepository;
    }

    protected override async Task<CustomerAdoptionOutput> ApplyInternalLogicAsync(CustomerAdoptionInput input, CancellationToken ct)
    {
        var existingClient = await _clientRepository.GetByCpfAsync(input.Cpf, ct);
        if (existingClient != null)
        {
            throw new InvalidOperationException("CPF ja cadastrado no sistema.");
        }

        var client = new Client(
            input.Name,
            input.Cpf,
            input.Email,
            input.MensalValue);

        client = await _clientRepository.AddAsync(client, ct); 

        var graphicalAccount = new GraphicalAccount(
            client.Id,
            AccountType.Filhote);
        
        graphicalAccount = await _graphicalAccountRepository.AddAndGenerateNumberAsync(graphicalAccount, ct);

        return new CustomerAdoptionOutput()
        {
            ClientId = client.Id,
            Cpf = client.Cpf.Number,
            Email = client.Email.Email,
            MensalValue = client.MonthlyInvestment,
            Name = client.Name,
            AdoptionDate = DateTime.UtcNow,
            IsActive = client.IsActive,
            GraphicalAccount = new GraphicalAccountDTO()
            {
                Id = graphicalAccount.Id,
                AccountNumber = graphicalAccount.AccountNumber,
                AccountType = graphicalAccount.AccountType.ToString(),
                CreatedAt = graphicalAccount.CreatedAt
            },
        };
    }
}