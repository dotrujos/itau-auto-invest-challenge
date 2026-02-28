using System.Text.Json.Serialization;
using Itau.AutoInvest.Application.DTOs;

namespace Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;

public class CustomerAdoptionOutput
{
    [JsonPropertyName("clienteId")]
    public long ClientId { get; set; }
    
    [JsonPropertyName("nome")]
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("valorMensal")]
    public decimal MensalValue { get; set; }
    
    [JsonPropertyName("ativo")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("dataAdesao")]
    public DateTime AdoptionDate { get; set; }
    
    [JsonPropertyName("contaGrafica")]
    public GraphicalAccountDTO GraphicalAccount { get; set; }
}