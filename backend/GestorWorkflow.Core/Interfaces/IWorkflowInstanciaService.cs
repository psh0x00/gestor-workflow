using GestorWorkflow.Core.DTO;

namespace GestorWorkflow.Core.Interfaces;

public interface IWorkflowInstanciaService
{
    Task<WorkflowInstanciaDTO> CriarWorkflowInstanciaAsync(CriarWorkflowInstanciaDTO dto);
    Task<WorkflowInstanciaDTO> ObterWorkflowInstanciaPorIdAsync(int id);
    Task<IEnumerable<WorkflowInstanciaDTO>> ObterTodasWorkflowInstanciasAsync();
    Task<ResultadoOperacaoResponse<RegistoTransicaoDTO>> ExecutarTransicaoAsync(ExecutarTransicaoDTO dto);
    Task<WorkflowInstanciaDTO> AtualizarWorkflowInstanciaAsync(int id, AtualizarWorkflowInstanciaDTO dto);
} 