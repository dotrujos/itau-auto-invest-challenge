using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("Cotacoes")]
public class StockQuoteTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("DataPregao")] 
    public DateTime PreachDate { get; set; }

    public string Ticker { get; set; }

    [Column("PrecoAbertura")]
    public decimal OpeningPrice { get; set; }

    [Column("PrecoFechamento")]
    public decimal ClosingPrice { get; set; }

    [Column("PrecoMaximo")]
    public decimal MaximumPrice { get; set; }

    [Column("PrecoMinimo")]
    public decimal MinimumPrice { get; set; }
}