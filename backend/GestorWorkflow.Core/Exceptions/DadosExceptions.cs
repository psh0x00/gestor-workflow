namespace GestorWorkflow.Core.Exceptions;

public class DadosCorruptosException : WorkflowExceptionBase
{
    public DadosCorruptosException(string tabela, int id, string motivo)
        : base("DADOS_CORROMPIDOS", $"Dados corrompidos na tabela '{tabela}' para o ID {id}: {motivo}")
    {
    }
}

public class BaseDadosIndisponivelException : WorkflowExceptionBase
{
    public BaseDadosIndisponivelException(string motivo)
        : base("BASE_DADOS_INDISPONIVEL", $"Base de dados indisponível: {motivo}")
    {
    }
}