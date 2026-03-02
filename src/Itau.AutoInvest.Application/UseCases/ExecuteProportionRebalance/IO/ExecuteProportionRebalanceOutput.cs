namespace Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.IO;

public record ExecuteProportionRebalanceOutput(
    DateTime ExecutedAt,
    int ClientsRebalanced,
    decimal TotalSalesValue,
    decimal TotalInvestedValue,
    string Message
);
