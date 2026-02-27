using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("Custodias")]
public class CustodiesTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    [Column("ContaGraficaId")]
    public long GraphicalAccountId { get; set; }
    public GraphicalAccountsTable GraphicalAccount { get; set; }
    
    [Required]
    public string Ticker { get; set; }
    
    [Required]
    [Column("Quantidade")]
    public int Quantity { get; set; }
    
    [Required]
    [Column("PrecoMedio")]
    public decimal AvaragePrice { get; set; }
    
    [Required]
    [Column("DataUltimaAtualizacao")]
    public DateTime LastUpdate { get; set; }
}