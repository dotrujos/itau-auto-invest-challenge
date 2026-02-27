using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("Rebalanceamentos")]
public class RebalancesTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    [Column("ClienteId")]
    public long ClientId { get; set; }
    public ClientsTable Client { get; set; }

    [Required]
    [Column("Tipo")]
    public RebalanceType RebalanceType;

    [Required]
    [Column("TickerVendido")]
    public string TickerSold { get; set; } = string.Empty;
    
    [Required]
    [Column("TickerComprado")]
    public string TickerPurchased { get; set; } = string.Empty;
    
    [Required]
    [Column("ValorVenda")]
    public decimal SalesValue { get; set; }
    
    [Required]
    [Column("DataRebalanceamento")]
    public DateTime DateRebalancing { get; set; }
}