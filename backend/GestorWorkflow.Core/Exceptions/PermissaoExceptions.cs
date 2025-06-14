namespace GestorWorkflow.Core.Exceptions;

public class PermissaoNegadaException : WorkflowExceptionBase
{
    public PermissaoNegadaException(int utilizadorId, int transicaoId)
        : base("PERMISSAO_NEGADA", $"Utilizador {utilizadorId} não tem permissão para executar a transição {transicaoId}.")
    {
    }
}

public class PermissaoNaoEncontradaException : WorkflowExceptionBase
{
    public PermissaoNaoEncontradaException(int permissaoId)
        : base("PERMISSAO_NAO_ENCONTRADA", $"Permissão com ID {permissaoId} não foi encontrada.")
    {
    }
}

public class PermissaoJaExisteException : WorkflowExceptionBase
{
    public PermissaoJaExisteException(string nome, int transicaoId)
        : base("PERMISSAO_JA_EXISTE", $"Já existe uma permissão com o nome '{nome}' para a transição {transicaoId}.")
    {
    }
}