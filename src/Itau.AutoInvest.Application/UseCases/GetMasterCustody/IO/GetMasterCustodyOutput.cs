namespace Itau.AutoInvest.Application.UseCases.GetMasterCustody.IO;

public record GetMasterCustodyOutput(
    ContaMasterOutput ContaMaster,
    IEnumerable<CustodiaMasterOutput> Custodia,
    decimal ValorTotalResiduo);

public record ContaMasterOutput(
    long Id,
    string NumeroConta,
    string Tipo);

public record CustodiaMasterOutput(
    string Ticker,
    int Quantidade,
    decimal PrecoMedio,
    decimal ValorAtual,
    string Origem);
