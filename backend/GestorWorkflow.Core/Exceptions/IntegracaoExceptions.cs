namespace GestorWorkflow.Core.Exceptions;

public class IntegracaoWorkflowException : WorkflowExceptionBase
{
    public IntegracaoWorkflowException(string sistema, string operacao, string motivo)
        : base("INTEGRACAO_WORKFLOW_FALHOU", $"Integração com {sistema} falhou na operação '{operacao}': {motivo}")
    {
    }
}

public class TimeoutWorkflowException : WorkflowExceptionBase
{
    public TimeoutWorkflowException(string operacao, int tempoLimiteSegundos)
        : base("TIMEOUT_WORKFLOW", $"Operação '{operacao}' excedeu o tempo limite de {tempoLimiteSegundos} segundos.")
    {
    }
}