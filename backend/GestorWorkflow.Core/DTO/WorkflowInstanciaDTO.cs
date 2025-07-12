using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.DTO;

public class WorkflowInstanciaDTO
{
    public int Id { get; set; }
    public int WorkflowModeloId { get; set; }
    public StatusWorkflowEntity Status { get; set; }
    public int? EstadoAtualId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? IniciadoPorId { get; set; }

    public List<RegistoTransicaoDTO> HistoricoTransicoes { get; set; } = new();

    // Propriedades adicionais para melhor visualização
    public string? NomeWorkflowModelo { get; set; }
    public string? NomeEstadoAtual { get; set; }
    public string? NomeIniciador { get; set; }
    public string StatusDescricao => Status.ToString();
}

public class CriarWorkflowInstanciaDTO
{
    public int WorkflowModeloId { get; set; }
    public int EstadoInicialId { get; set; }
    public int? IniciadoPorId { get; set; }
    public List<EquipaAtribuicaoDTO>? Equipa { get; set; } // NOVO: atribuições por função
}

public class AtualizarWorkflowInstanciaDTO
{
    public StatusWorkflowEntity? Status { get; set; }
}