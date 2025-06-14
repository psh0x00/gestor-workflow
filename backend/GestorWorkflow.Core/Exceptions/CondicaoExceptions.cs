namespace GestorWorkflow.Core.Exceptions;

public class PreCondicaoNaoAtendidaException : WorkflowExceptionBase
{
    public PreCondicaoNaoAtendidaException(int preCondicaoId, string motivo)
        : base("PRE_CONDICAO_NAO_ATENDIDA", $"Pré-condição {preCondicaoId} não foi atendida: {motivo}")
    {
    }
}

public class PosCondicaoFalhouException : WorkflowExceptionBase
{
    public PosCondicaoFalhouException(int posCondicaoId, string motivo)
        : base("POS_CONDICAO_FALHOU", $"Pós-condição {posCondicaoId} falhou: {motivo}")
    {
    }
}

public class CondicaoNaoEncontradaException : WorkflowExceptionBase
{
    public CondicaoNaoEncontradaException(int condicaoId, string tipo)
        : base("CONDICAO_NAO_ENCONTRADA", $"{tipo} com ID {condicaoId} não foi encontrada.")
    {
    }
}