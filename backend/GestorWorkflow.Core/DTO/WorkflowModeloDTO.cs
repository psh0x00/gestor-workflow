namespace GestorWorkflow.Core.DTO;

public class WorkflowModeloDTO
{
    // Propriedades para última pré e pós-condição do modelo
    public string? UltimaPreCondicao { get; set; }
    public string? UltimaPosCondicao { get; set; }
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Versao { get; set; } = string.Empty;
    public int? EstadoInicialId { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAlteracao { get; set; }
    public int CriadoPorId { get; set; }
    public int? AlteradoPorId { get; set; }

    public List<TransicaoDTO> Transicoes { get; set; } = new();
    public List<EstadoDTO> Estados { get; set; } = new();

    // Propriedades adicionais para melhor visualização
    public string? NomeEstadoInicial { get; set; }
    public string? NomeCriador { get; set; }
    public string? NomeAlterador { get; set; }
}

public class CriarWorkflowModeloDTO
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int EstadoInicialId { get; set; }
    public int CriadoPorId { get; set; }
    public List<CriarEstadoDTO> Estados { get; set; } = new();
    public List<CriarTransicaoDTO> Transicoes { get; set; } = new();
}

public class AtualizarWorkflowModeloDTO
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Versao { get; set; }
    public int? AlteradoPorId { get; set; }
    public bool? Ativo { get; set; }
}