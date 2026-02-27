using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Tables;

[Table("Clientes")]
public class ClientsTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public BigInteger Id { get; set; }

    [Required]
    [Column("Nome")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Column("CPF")] 
    public string Cpf { get; set; } = string.Empty;

    [Required]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Column("ValorMensal")]
    public decimal MonthlyValue { get; set; }
    
    [Required]
    [Column("Ativo")]
    public bool IsActive { get; set; }

    public GraphicalAccountsTable GraphicalAccountTable { get; set; }

    [Column("DataAcesso")]
    public DateTime AccessDate { get; set; }
}