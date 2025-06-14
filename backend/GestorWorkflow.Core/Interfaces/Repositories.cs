using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.Interfaces
{

    // Interface para repositório de Estados
    public interface IEstadoRepository
    {
        Task<EstadoEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<EstadoEntity>> ObterTodosAsync();
        Task<IEnumerable<EstadoEntity>> ObterAtivosAsync();
        Task<IEnumerable<EstadoEntity>> ObterPorTipoAsync(TipoEstadoEntity tipo);
        Task<EstadoEntity> CriarAsync(EstadoEntity estadoEntity);
        Task<EstadoEntity> AtualizarAsync(EstadoEntity estadoEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Transições
    public interface ITransicaoRepository
    {
        Task<TransicaoEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<TransicaoEntity>> ObterTodosAsync();
        Task<IEnumerable<TransicaoEntity>> ObterPorEstadoOrigemAsync(int estadoOrigemId);
        Task<IEnumerable<TransicaoEntity>> ObterPorEstadoDestinoAsync(int estadoDestinoId);
        Task<IEnumerable<TransicaoEntity>> ObterTransicoesIniciaisAsync();
        Task<TransicaoEntity> CriarAsync(TransicaoEntity transicaoEntity);
        Task<TransicaoEntity> AtualizarAsync(TransicaoEntity transicaoEntity);
        Task<bool> ExisteAsync(int id);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Workflow Modelo
    public interface IWorkflowModeloRepository
    {
        Task<WorkflowModeloEntity?> ObterPorIdAsync(int id);
        Task<WorkflowModeloEntity?> ObterComDetalhesAsync(int id);
        Task<IEnumerable<WorkflowModeloEntity>> ObterTodosAsync();
        Task<IEnumerable<WorkflowModeloEntity>> ObterAtivosAsync();
        Task<IEnumerable<WorkflowModeloEntity>> ObterPorCriadorAsync(int criadorId);
        Task<WorkflowModeloEntity> CriarAsync(WorkflowModeloEntity workflowModeloEntity);
        Task<WorkflowModeloEntity> AtualizarAsync(WorkflowModeloEntity workflowModeloEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Workflow Instância
    public interface IWorkflowInstanciaRepository
    {
        Task<WorkflowInstanciaEntity?> ObterPorIdAsync(int id);
        Task<WorkflowInstanciaEntity?> ObterComHistoricoAsync(int id);
        Task<IEnumerable<WorkflowInstanciaEntity>> ObterTodosAsync();
        Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorModeloAsync(int workflowModeloId);
        Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorStatusAsync(StatusWorkflowEntity status);
        Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorIniciadorAsync(int iniciadorId);
        Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorTransicaoAsync(int transicaoId);
        Task<WorkflowInstanciaEntity> CriarAsync(WorkflowInstanciaEntity workflowInstanciaEntity);
        Task<WorkflowInstanciaEntity> AtualizarAsync(WorkflowInstanciaEntity workflowInstanciaEntity);
        Task<bool> ExisteAsync(int id);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Utilizadores
    public interface IUtilizadorRepository
    {
        Task<UtilizadorEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<UtilizadorEntity>> ObterTodosAsync();
        Task<IEnumerable<UtilizadorEntity>> ObterPorFuncaoAsync(string funcao);
        Task<IEnumerable<UtilizadorEntity>> ObterComPermissaoAsync(int permissaoId);
        Task<UtilizadorEntity> CriarAsync(UtilizadorEntity utilizadorEntity);
        Task<UtilizadorEntity> AtualizarAsync(UtilizadorEntity utilizadorEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Permissões
    public interface IPermissaoRepository
    {
        Task<PermissaoEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<PermissaoEntity>> ObterTodosAsync();
        Task<IEnumerable<PermissaoEntity>> ObterPorTransicaoAsync(int transicaoId);
        Task<PermissaoEntity> CriarAsync(PermissaoEntity permissaoEntity);
        Task<PermissaoEntity> AtualizarAsync(PermissaoEntity permissaoEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Pré-condições
    public interface IPreCondicaoRepository
    {
        Task<PreCondicaoEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<PreCondicaoEntity>> ObterTodosAsync();
        Task<IEnumerable<PreCondicaoEntity>> ObterAtivosAsync();
        Task<PreCondicaoEntity> CriarAsync(PreCondicaoEntity preCondicaoEntity);
        Task<PreCondicaoEntity> AtualizarAsync(PreCondicaoEntity preCondicaoEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }

    // Interface para repositório de Pós-condições
    public interface IPosCondicaoRepository
    {
        Task<PosCondicaoEntity?> ObterPorIdAsync(int id);
        Task<IEnumerable<PosCondicaoEntity>> ObterTodosAsync();
        Task<IEnumerable<PosCondicaoEntity>> ObterAtivosAsync();
        Task<PosCondicaoEntity> CriarAsync(PosCondicaoEntity posCondicaoEntity);
        Task<PosCondicaoEntity> AtualizarAsync(PosCondicaoEntity posCondicaoEntity);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteNomeAsync(string nome, int? excludeId = null);
        Task RemoverAsync(int id);
    }
}