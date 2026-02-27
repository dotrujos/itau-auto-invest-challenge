using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("Distribuicoes")]
public class DistributionsTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    [Column("OrdemCompraId")]
    public long BuyOrderId { get; set; }
    public BuyOrderTable BuyOrder { get; set; }
    
    [Required]
    [Column("CustodiaFilhoteId")]
    public long CustodyId { get; set; }
    public CustodiesTable Custody { get; set; }

    [Required]
    public string Ticker { get; set; } = string.Empty;
    
    [Required]
    [Column("Quantidade")]
    public int Quantity { get; set; }
    
    [Required]
    [Column("PrecoUnitario")]
    public decimal UnitPrice { get; set; }
    
    [Required]
    [Column("DataDistribuicao")]
    public DateTime DistributionDate { get; set; }
}