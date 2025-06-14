namespace GestorWorkflow.Core.Exceptions;

public class EstadoNaoEncontradoException : WorkflowExceptionBase
{
    public EstadoNaoEncontradoException(int estadoId)
        : base("ESTADO_NAO_ENCONTRADO", $"Estado com ID {estadoId} não foi encontrado.")
    {
    }
}

public class EstadoNomeJaExisteException : WorkflowExceptionBase
{
    public EstadoNomeJaExisteException(string nome)
        : base("ESTADO_NOME_DUPLICADO", $"Já existe um estado com o nome '{nome}'.")
    {
    }
}

public class EstadoInvalidoException : WorkflowExceptionBase
{
    public EstadoInvalidoException(string motivo)
        : base("ESTADO_INVALIDO", $"Estado inválido: {motivo}")
    {
    }
}

public class EstadoEmUsoException : WorkflowExceptionBase
{
    public EstadoEmUsoException(int estadoId)
        : base("ESTADO_EM_USO", $"O estado com ID {estadoId} está sendo usado em transições e não pode ser removido.")
    {
    }
}