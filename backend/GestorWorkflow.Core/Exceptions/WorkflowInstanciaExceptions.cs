namespace GestorWorkflow.Core.Exceptions;

public class WorkflowInstanciaNaoEncontradaException : WorkflowExceptionBase
{
    public WorkflowInstanciaNaoEncontradaException(int instanciaId)
        : base("INSTANCIA_WORKFLOW_NAO_ENCONTRADA", $"Instância de workflow com ID {instanciaId} não foi encontrada.")
    {
    }
}

public class WorkflowInstanciaInativaException : WorkflowExceptionBase
{
    public WorkflowInstanciaInativaException(int instanciaId)
        : base("INSTANCIA_WORKFLOW_INATIVA", $"A instância de workflow {instanciaId} não está ativa.")
    {
    }
}

public class WorkflowInstanciaJaConcluidaException : WorkflowExceptionBase
{
    public WorkflowInstanciaJaConcluidaException(int instanciaId)
        : base("INSTANCIA_WORKFLOW_JA_CONCLUIDA", $"A instância de workflow {instanciaId} já foi concluída.")
    {
    }
}

public class WorkflowInstanciaSuspensaException : WorkflowExceptionBase
{
    public WorkflowInstanciaSuspensaException(int instanciaId)
        : base("INSTANCIA_WORKFLOW_SUSPENSA", $"A instância de workflow {instanciaId} está suspensa.")
    {
    }
}