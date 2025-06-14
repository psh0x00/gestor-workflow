using GestorWorkflow.Core.DTO;

namespace GestorWorkflow.Core.Interfaces;

public interface IWorkflowModeloService
{
    Task<WorkflowModeloDTO> CriarWorkflowModeloAsync(CriarWorkflowModeloDTO dto);
    Task<WorkflowModeloDTO> ObterWorkflowModeloPorIdAsync(int id);
    Task<IEnumerable<WorkflowModeloDTO>> ObterTodosWorkflowModelosAsync();
    Task<WorkflowModeloDTO> AtualizarWorkflowModeloAsync(int id, AtualizarWorkflowModeloDTO dto);
    Task RemoverWorkflowModeloAsync(int id);
} 