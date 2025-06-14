namespace GestorWorkflow.Core.DTO;

public class PermissaoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TransicaoId { get; set; }

    // Propriedade de navegação
    public string? NomeTransicao { get; set; }
}

public class CriarPermissaoDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TransicaoId { get; set; }
}

public class AtualizarPermissaoDTO
{
    public string? Descricao { get; set; }
}