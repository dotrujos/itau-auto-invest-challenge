namespace Itau.AutoInvest.Domain.Exceptions;

// Erros de Cliente
public class DuplicateCpfException : BaseDomainException
{
    public DuplicateCpfException() 
        : base("CPF ja cadastrado no sistema.", "CLIENTE_CPF_DUPLICADO") { }
}

public class InvalidMonthlyValueException : BaseDomainException
{
    public InvalidMonthlyValueException() 
        : base("O valor mensal minimo e de R$ 100,00.", "VALOR_MENSAL_INVALIDO") { }
}

public class ClientAlreadyInactiveException : BaseDomainException
{
    public ClientAlreadyInactiveException() 
        : base("Cliente ja havia saido do produto.", "CLIENTE_JA_INATIVO") { }
}

public class ClientNotFoundException : BaseDomainException
{
    public ClientNotFoundException() 
        : base("Cliente nao encontrado.", "CLIENTE_NAO_ENCONTRADO") { }
}

// Erros de Cesta / Admin
public class InvalidBasketPercentageException : BaseDomainException
{
    public InvalidBasketPercentageException(decimal sum) 
        : base($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {sum}%.", "PERCENTUAIS_INVALIDOS") { }
}

public class InvalidBasketQuantityException : BaseDomainException
{
    public InvalidBasketQuantityException(int count) 
        : base($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {count}.", "QUANTIDADE_ATIVOS_INVALIDA") { }
}

public class BasketNotFoundException : BaseDomainException
{
    public BasketNotFoundException() 
        : base("Nenhuma cesta ativa encontrada.", "CESTA_NAO_ENCONTRADA") { }
}

// Erros de Motor / Cotações
public class QuoteNotFoundException : BaseDomainException
{
    public QuoteNotFoundException(DateTime date) 
        : base($"Arquivo COTAHIST nao encontrado para a data {date:dd/MM/yyyy}.", "COTACAO_NAO_ENCONTRADA") { }
}

public class PurchaseAlreadyExecutedException : BaseDomainException
{
    public PurchaseAlreadyExecutedException(DateTime date) 
        : base($"Compra ja foi executada para a data {date:dd/MM/yyyy}.", "COMPRA_JA_EXECUTADA") { }
}

// Erros de Infra/Kafka
public class KafkaUnavailableException : BaseDomainException
{
    public KafkaUnavailableException() 
        : base("Erro ao publicar no topico Kafka.", "KAFKA_INDISPONIVEL") { }
}
