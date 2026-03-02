namespace Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.IO;

public record ExecuteProportionRebalanceInput(decimal ThresholdPercentage = 5m);
