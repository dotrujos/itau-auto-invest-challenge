using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.DTOs;

public class GraphicalAccountDTO
{
    public long Id { get; set; }
    
    [JsonPropertyName("numeroConta")]
    public string AccountNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("tipo")]
    public string AccountType { get; set; } = string.Empty;
    
    [JsonPropertyName("dataCriacao")]
    public DateTime CreatedAt { get; set; }
}