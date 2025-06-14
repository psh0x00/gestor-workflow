namespace GestorWorkflow.Core.DTO;

public class UtilizadorDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public List<int> PermissoesIds { get; set; } = new();

    // Propriedades adicionais para melhor visualização
    public List<string> NomesPermissoes { get; set; } = new();
}

public class CriarUtilizadorDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public List<int> PermissoesIds { get; set; } = new();
}

public class AtualizarUtilizadorDTO
{
    public string? Nome { get; set; }
    public string? Funcao { get; set; }
    public List<int>? PermissoesIds { get; set; }
}