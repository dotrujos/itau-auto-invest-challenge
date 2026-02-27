using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("ItensCesta")]
public class BasketItemsTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public BigInteger Id { get; set; }
    
    [Required]
    [Column("CestaId")]
    public BigInteger ParentBasketId { get; set; }
    public BasketRecommendationTable ParentBasket { get; set; }
    
    [Required]
    public string Ticker { get; set; }
    
    [Required]
    [Column("Percentual")]
    public decimal Percentage { get; set; }
}