using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.CustomerAdoption.Implementations;

public class CustomerAdoptionImpl : CustomerAdoption
{
    private readonly IGraphicalAccountRepository _graphicalAccountRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerAdoptionImpl> _logger;
    
    public CustomerAdoptionImpl(
        IGraphicalAccountRepository graphicalAccountRepository, 
        IClientRepository clientRepository, 
        IUnitOfWork unitOfWork,
        ILogger<CustomerAdoptionImpl> logger)
    {
        _graphicalAccountRepository = graphicalAccountRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task<CustomerAdoptionOutput> ApplyInternalLogicAsync(CustomerAdoptionInput input, CancellationToken ct)
    {
        _logger.LogInformation("Iniciando processo de adesão para o cliente: {Name}, CPF: {Cpf}", input.Name, input.Cpf);

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var existingClient = await _clientRepository.GetByCpfAsync(input.Cpf, ct);
            if (existingClient != null)
            {
                _logger.LogWarning("Tentativa de adesão com CPF já cadastrado: {Cpf}", input.Cpf);
                throw new DuplicateCpfException();
            }

            var client = new Client(
                input.Name,
                input.Cpf,
                input.Email,
                input.MensalValue);

            client = await _clientRepository.AddAsync(client, ct); 
            await _unitOfWork.SaveChangesAsync(ct);

            var graphicalAccount = new GraphicalAccount(
                client.Id,
                AccountType.Filhote);
            
            graphicalAccount = await _graphicalAccountRepository.AddAndGenerateNumberAsync(graphicalAccount, ct);

            await _unitOfWork.CommitAsync(ct);

            _logger.LogInformation("Cliente {ClientId} aderido com sucesso. Conta: {AccountNumber}", client.Id, graphicalAccount.AccountNumber);

            return new CustomerAdoptionOutput()
            {
                ClientId = client.Id,
                Cpf = client.Cpf.Number,
                Email = client.Email.Email,
                MensalValue = client.MonthlyInvestment,
                Name = client.Name,
                AdoptionDate = DateTime.UtcNow,
                IsActive = client.IsActive,
                GraphicalAccount = new GraphicalAccountOutput(
                    graphicalAccount.Id,
                    graphicalAccount.AccountNumber,
                    graphicalAccount.AccountType.ToString(),
                    graphicalAccount.CreatedAt)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar adesão do cliente com CPF {Cpf}", input.Cpf);
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}