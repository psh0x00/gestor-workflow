namespace GestorWorkflow.Core.Exceptions;

public class UtilizadorNaoEncontradoException : WorkflowExceptionBase
{
    public UtilizadorNaoEncontradoException(int utilizadorId)
        : base("UTILIZADOR_NAO_ENCONTRADO", $"Utilizador com ID {utilizadorId} não foi encontrado.")
    {
    }
}

public class UtilizadorNomeJaExisteException : WorkflowExceptionBase
{
    public UtilizadorNomeJaExisteException(string nome)
        : base("UTILIZADOR_NOME_DUPLICADO", $"Já existe um utilizador com o nome '{nome}'.")
    {
    }
}

public class UtilizadorInvalidoException : WorkflowExceptionBase
{
    public UtilizadorInvalidoException(string motivo)
        : base("UTILIZADOR_INVALIDO", $"Utilizador inválido: {motivo}")
    {
    }
}