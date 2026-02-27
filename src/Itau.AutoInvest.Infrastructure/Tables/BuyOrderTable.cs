using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("OrdensCompra")]
public class BuyOrderTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public BigInteger Id { get; set; }
    
    [Required]
    [Column("ContaMasterId")]
    public BigInteger GraphicalAccountId { get; set; }
    public GraphicalAccountsTable GraphicalAccount { get; set; }

    [Required]
    public String Ticker { get; set; } = string.Empty;
    
    [Required]
    [Column("Quantidade")]
    public int Quantity { get; set; }
    
    [Required]
    [Column("PrecoUnitario")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column("TipoMercado")]
    public MarketType MarketType;
    
    [Required]
    [Column("DataExecucao")]
    public DateTime ExecutionDate;
}