namespace GestorWorkflow.Core.DTO;

public class PreCondicaoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? CondicaoSql { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? CriadoPorId { get; set; }
}

public class CriarPreCondicaoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? CondicaoSql { get; set; }
    public int? CriadoPorId { get; set; }
}

public class AtualizarPreCondicaoDTO
{
    public string? Descricao { get; set; }
    public string? CondicaoSql { get; set; }
    public bool? Ativo { get; set; }
}