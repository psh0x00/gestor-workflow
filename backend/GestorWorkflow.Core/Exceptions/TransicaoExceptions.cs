namespace GestorWorkflow.Core.Exceptions;

public class TransicaoNaoEncontradaException : WorkflowExceptionBase
{
    public TransicaoNaoEncontradaException(int transicaoId)
        : base("TRANSICAO_NAO_ENCONTRADA", $"Transição com ID {transicaoId} não foi encontrada.")
    {
    }
}

public class TransicaoNaoPermitidaException : WorkflowExceptionBase
{
    public TransicaoNaoPermitidaException(int estadoAtualId, int estadoDestinoId)
        : base("TRANSICAO_NAO_PERMITIDA", $"Transição do estado {estadoAtualId} para o estado {estadoDestinoId} não é permitida.")
    {
    }
}

public class TransicaoInvalidaException : WorkflowExceptionBase
{
    public TransicaoInvalidaException(string motivo)
        : base("TRANSICAO_INVALIDA", $"Transição inválida: {motivo}")
    {
    }
}