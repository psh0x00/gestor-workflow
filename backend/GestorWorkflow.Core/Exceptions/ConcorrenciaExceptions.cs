namespace GestorWorkflow.Core.Exceptions;

public class ConcorrenciaWorkflowException : WorkflowExceptionBase
{
    public ConcorrenciaWorkflowException(int instanciaId)
        : base("CONCORRENCIA_WORKFLOW", $"Conflito de concorrência detectado na instância {instanciaId}. Tente novamente.")
    {
    }
}

public class RecursoEmUsoException : WorkflowExceptionBase
{
    public RecursoEmUsoException(string recurso, int id)
        : base("RECURSO_EM_USO", $"{recurso} com ID {id} está sendo usado e não pode ser removido.")
    {
    }
}