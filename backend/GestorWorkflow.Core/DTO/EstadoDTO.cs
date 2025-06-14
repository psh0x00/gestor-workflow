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
}

public class CriarEstadoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoEstadoEntity Tipo { get; set; }
    public string? CorHexadecimal { get; set; }
    public int? CriadoPorId { get; set; }
}

public class AtualizarEstadoDTO
{
    public string? Descricao { get; set; }
    public string? CorHexadecimal { get; set; }
    public bool? Ativo { get; set; }
}