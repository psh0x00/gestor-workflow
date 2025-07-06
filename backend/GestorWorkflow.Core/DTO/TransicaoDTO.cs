using System.Text.Json.Serialization;

namespace GestorWorkflow.Core.DTO;

public class TransicaoDTO
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public int? EstadoOrigemId { get; set; }
    public int EstadoDestinoId { get; set; }
    public int? PreCondicaoId { get; set; }
    public int? PosCondicaoId { get; set; }
    public List<int> PermissoesIds { get; set; } = new();

    // Propriedades de navegação para melhor visualização
    public string? NomeEstadoOrigem { get; set; }
    public string? NomeEstadoDestino { get; set; }
    public string? NomePreCondicao { get; set; }
    public string? NomePosCondicao { get; set; }
}

public class CriarTransicaoDTO
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public int? EstadoOrigemId { get; set; }
    public int EstadoDestinoId { get; set; }
    public int? PreCondicaoId { get; set; }
    public int? PosCondicaoId { get; set; }
    public List<int> PermissoesIds { get; set; } = new();

    // Propriedades para criação por nome
    [JsonPropertyName("origem")]
    public string? NomeEstadoOrigem { get; set; }
    [JsonPropertyName("destino")]
    public string? NomeEstadoDestino { get; set; }
}

public class AtualizarTransicaoDTO
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public int? PreCondicaoId { get; set; }
    public int? PosCondicaoId { get; set; }
    public List<int>? PermissoesIds { get; set; }
}