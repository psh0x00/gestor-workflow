namespace GestorWorkflow.Core.Exceptions;

public class WorkflowModeloNaoEncontradoException : WorkflowExceptionBase
{
    public WorkflowModeloNaoEncontradoException(int modeloId)
        : base("MODELO_WORKFLOW_NAO_ENCONTRADO", $"Modelo de workflow com ID {modeloId} não foi encontrado.")
    {
    }
}

public class WorkflowModeloNomeJaExisteException : WorkflowExceptionBase
{
    public WorkflowModeloNomeJaExisteException(string nome)
        : base("MODELO_WORKFLOW_NOME_DUPLICADO", $"Já existe um modelo de workflow com o nome '{nome}'.")
    {
    }
}

public class WorkflowModeloInvalidoException : WorkflowExceptionBase
{
    public WorkflowModeloInvalidoException(string motivo)
        : base("MODELO_WORKFLOW_INVALIDO", $"Modelo de workflow inválido: {motivo}")
    {
    }
}

public class WorkflowModeloComInstanciasAtivasException : WorkflowExceptionBase
{
    public WorkflowModeloComInstanciasAtivasException(int modeloId)
        : base("WORKFLOW_MODELO_COM_INSTANCIAS_ATIVAS", $"O modelo de workflow com ID {modeloId} possui instâncias ativas e não pode ser removido.")
    {
    }
}