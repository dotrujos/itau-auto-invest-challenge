using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;

public class CustomerAdoptionInput
{
    [JsonPropertyName("nome")]
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("valorMensal")]
    public decimal MensalValue { get; set; }
}