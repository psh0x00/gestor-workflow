using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.DTO;

public class EstadoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoEstadoEntity Tipo { get; set; }
    public string? CorHexadecimal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? CriadoPorId { get; set; }
    public List<string> Funcoes { get; set; } = new();

    // Novas propriedades para expor condições
    public string? PreCondicao { get; set; }
    public string? PosCondicao { get; set; }
}

public class CriarEstadoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoEstadoEntity Tipo { get; set; }
    public string? CorHexadecimal { get; set; }
    public int? CriadoPorId { get; set; }
    public bool IsInicial { get; set; }
    public bool IsFinal { get; set; }
    public List<string> Funcoes { get; set; } = new();
    public string? PreCondicao { get; set; }
    public string? PosCondicao { get; set; }
}

public class AtualizarEstadoDTO
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? CorHexadecimal { get; set; }
    public bool? Ativo { get; set; }
    public List<string>? Funcoes { get; set; }
}