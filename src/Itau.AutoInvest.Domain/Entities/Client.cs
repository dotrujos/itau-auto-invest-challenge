using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.Domain.ValueObjects;

namespace Itau.AutoInvest.Domain.Entities;

public class Client
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public CpfValueObject Cpf { get; private set; }
    public EmailValueObject Email { get; private set; }
    public decimal MonthlyInvestment { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    
    private Client() { }
    
    public Client(string name, string cpf, string email, decimal monthlyInvestment)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do cliente nao pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(cpf)) 
            throw new ArgumentException("O CPF do cliente nao pode ser vazio.", nameof(cpf));
        if (string.IsNullOrWhiteSpace(email)) 
            throw new ArgumentException("O e-mail do cliente nao pode ser vazio.", nameof(email));
        
        if (monthlyInvestment < 100) 
            throw new InvalidMonthlyValueException();

        Name = name;
        Cpf = new CpfValueObject(cpf);
        Email = new EmailValueObject(email);
        MonthlyInvestment = monthlyInvestment;
        IsActive = true;
        RegistrationDate = DateTime.UtcNow; 
    }
    
    public Client(long id, string name, CpfValueObject cpf, EmailValueObject email, decimal monthlyInvestment, bool isActive, DateTime registrationDate)
    {
        Id = id;
        Name = name;
        Cpf = cpf;
        Email = email;
        MonthlyInvestment = monthlyInvestment;
        IsActive = isActive;
        RegistrationDate = registrationDate;
    }

    public void UpdateMonthlyInvestment(decimal newAmount)
    {
        if (newAmount < 100)
            throw new InvalidMonthlyValueException();
        
        MonthlyInvestment = newAmount;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
}
