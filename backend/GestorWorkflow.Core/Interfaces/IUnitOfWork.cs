using GestorWorkflow.Core.Entities;
namespace GestorWorkflow.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEstadoRepository Estados { get; }
    ITransicaoRepository Transicoes { get; }
    IWorkflowModeloRepository WorkflowModelos { get; }
    IWorkflowInstanciaRepository WorkflowInstancias { get; }
    IUtilizadorRepository Utilizadores { get; }
    IPermissaoRepository Permissoes { get; }
    IPreCondicaoRepository PreCondicoes { get; }
    IPosCondicaoRepository PosCondicoes { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<EstadoEntity> CriarEstadoAsync(EstadoEntity estadoEntity, int workflowModeloId);
}