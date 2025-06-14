using GestorWorkflow.Core.Entities;

namespace GestorWorkflow.Core.Interfaces;

public interface IWorkflowQueryRepository
{
    Task<IEnumerable<WorkflowInstanciaEntity>> GetWorkflowsEmAndamentoAsync();
    Task<IEnumerable<RegistoTransicaoEntity>> GetEstatisticasTransicoesAsync(DateTime dataInicio, DateTime dataFim);
    Task<Dictionary<string, int>> GetEstatisticasPorEstadoAsync(int workflowModeloId);
    Task<IEnumerable<WorkflowInstanciaEntity>> GetWorkflowsAtrasadosAsync(TimeSpan tempoLimite);
    Task<IEnumerable<UtilizadorEntity>> GetUtilizadoresMaisAtivosAsync(DateTime dataInicio, DateTime dataFim, int limite = 10);
    Task<decimal> GetTaxaSucessoWorkflowAsync(int workflowModeloId);
    Task<TimeSpan> GetTempoMedioConclusaoAsync(int workflowModeloId);
}