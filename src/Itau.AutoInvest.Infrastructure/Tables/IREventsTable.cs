using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("EventosIR")]
public class IREventsTable
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
    public IREventType EventType { get; set; }
    
    [Required]
    [Column("ValorBase")]
    public decimal BaseValue { get; set; }
    
    [Required]
    [Column("ValorIR")]
    public decimal IRValue { get; set; }
    
    [Required]
    [Column("PublicadoKafka")]
    public bool IsPublishedOnKafka { get; set; }
    
    [Required]
    [Column("DataEvento")]
    public DateTime EventDate { get; set; }
}