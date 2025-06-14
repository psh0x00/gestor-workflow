namespace GestorWorkflow.Core.DTO;

public class PosCondicaoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? AcaoSql { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? CriadoPorId { get; set; }
}

public class CriarPosCondicaoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? AcaoSql { get; set; }
    public int? CriadoPorId { get; set; }
}

public class AtualizarPosCondicaoDTO
{
    public string? Descricao { get; set; }
    public string? AcaoSql { get; set; }
    public bool? Ativo { get; set; }
}