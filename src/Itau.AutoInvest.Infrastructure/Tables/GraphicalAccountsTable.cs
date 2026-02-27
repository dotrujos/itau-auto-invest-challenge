using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("ContasGraficas")]
public class GraphicalAccountsTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    [Column("ClienteId")]
    public long ClientId { get; set; }

    public ClientsTable Client { get; set; }

    [Required]
    [Column("NumeroConta")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [Column("Tipo")]
    public AccountType AccountType;
    
    [Required]
    [Column("DataCriacao")]
    public DateTime CreatedAt { get; set; }
}