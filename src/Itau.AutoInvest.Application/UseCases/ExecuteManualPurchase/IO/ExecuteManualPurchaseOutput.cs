namespace Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;

public record ExecuteManualPurchaseOutput(
    DateTime DataExecucao,
    int TotalClientes,
    decimal TotalConsolidado,
    IEnumerable<OrdemCompraOutput> OrdensCompra,
    IEnumerable<DistribuicaoOutput> Distribuicoes,
    IEnumerable<ResiduoMasterOutput> ResiduosCustMaster,
    int EventosIRPublicados,
    string Mensagem);

public record OrdemCompraOutput(
    string Ticker,
    int QuantidadeTotal,
    IEnumerable<DetalheOrdemOutput> Detalhes,
    decimal PrecoUnitario,
    decimal ValorTotal);

public record DetalheOrdemOutput(
    string Tipo,
    string Ticker,
    int Quantidade);

public record DistribuicaoOutput(
    long ClienteId,
    string Nome,
    decimal ValorAporte,
    IEnumerable<AtivoDistribuidoOutput> Ativos);

public record AtivoDistribuidoOutput(
    string Ticker,
    int Quantidade);

public record ResiduoMasterOutput(
    string Ticker,
    int Quantidade);
