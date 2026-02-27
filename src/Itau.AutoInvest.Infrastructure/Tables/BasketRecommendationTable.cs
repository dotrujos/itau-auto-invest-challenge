using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("CestasRecomendacao")]
public class BasketRecommendationTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("Nome")] 
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("Ativa")]
    public bool IsActive { get; set; }
    
    [Required]
    [Column("DataCriacao")]
    public DateTime CreatedAt { get; set; }
    
    [Column("DataDesativacao")]
    public DateTime DeactivationDate { get; set; }
}