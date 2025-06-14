using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.Interfaces;

public interface IWorkflowService
{
    // Gestão de Estados
    Task<EstadoDTO> CriarEstadoAsync(CriarEstadoDTO dto);
    Task<EstadoDTO> ObterEstadoPorIdAsync(int id);
    Task<IEnumerable<EstadoDTO>> ObterTodosEstadosAsync();
    Task<EstadoDTO> AtualizarEstadoAsync(int id, AtualizarEstadoDTO dto);
    Task RemoverEstadoAsync(int id);

    // Gestão de Transições
    Task<TransicaoDTO> CriarTransicaoAsync(CriarTransicaoDTO dto);
    Task<TransicaoDTO> ObterTransicaoPorIdAsync(int id);
    Task<IEnumerable<TransicaoDTO>> ObterTodasTransicoesAsync();
    Task<TransicaoDTO> AtualizarTransicaoAsync(int id, AtualizarTransicaoDTO dto);
    Task RemoverTransicaoAsync(int id);

    // Gestão de Permissões
    Task<PermissaoDTO> CriarPermissaoAsync(CriarPermissaoDTO dto);
    Task<PermissaoDTO> ObterPermissaoPorIdAsync(int id);
    Task<IEnumerable<PermissaoDTO>> ObterTodasPermissoesAsync();
    Task<PermissaoDTO> AtualizarPermissaoAsync(int id, AtualizarPermissaoDTO dto);
    Task RemoverPermissaoAsync(int id);

    // Gestão de Workflows
    Task<WorkflowModeloDTO> CriarWorkflowModeloAsync(CriarWorkflowModeloDTO dto);
    Task<WorkflowModeloDTO> ObterWorkflowModeloPorIdAsync(int id);
    Task<IEnumerable<WorkflowModeloDTO>> ObterTodosWorkflowModelosAsync();
    Task<WorkflowModeloDTO> AtualizarWorkflowModeloAsync(int id, AtualizarWorkflowModeloDTO dto);
    Task RemoverWorkflowModeloAsync(int id);

    // Gestão de Instâncias
    Task<WorkflowInstanciaDTO> CriarWorkflowInstanciaAsync(CriarWorkflowInstanciaDTO dto);
    Task<WorkflowInstanciaDTO> ObterWorkflowInstanciaPorIdAsync(int id);
    Task<IEnumerable<WorkflowInstanciaDTO>> ObterTodasWorkflowInstanciasAsync();
    Task<ResultadoOperacaoResponse<RegistoTransicaoDTO>> ExecutarTransicaoAsync(ExecutarTransicaoDTO dto);
    Task<WorkflowInstanciaDTO> AtualizarWorkflowInstanciaAsync(int id, AtualizarWorkflowInstanciaDTO dto);

    // Validações e Utilitários
    Task<bool> ValidarTransicaoAsync(int workflowInstanciaId, int transicaoId, int? utilizadorId = null);
    Task<IEnumerable<TransicaoDTO>> ObterTransicoesPossiveisAsync(int workflowInstanciaId);
    Task<bool> UtilizadorTemPermissaoAsync(int utilizadorId, int permissaoId);
}