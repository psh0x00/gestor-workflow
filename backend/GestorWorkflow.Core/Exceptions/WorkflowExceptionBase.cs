using GestorWorkflow.Core.Interfaces;

namespace GestorWorkflow.Core.Exceptions;

public abstract class WorkflowExceptionBase : Exception, IWorkflowException
{
    public string Codigo { get; protected set; }
    public string Detalhes { get; protected set; }
    public DateTime DataOcorrencia { get; protected set; }

    protected WorkflowExceptionBase(string codigo, string mensagem) : base(mensagem)
    {
        Codigo = codigo;
        Detalhes = mensagem;
        DataOcorrencia = DateTime.UtcNow;
    }

    protected WorkflowExceptionBase(string codigo, string mensagem, Exception innerException)
        : base(mensagem, innerException)
    {
        Codigo = codigo;
        Detalhes = mensagem;
        DataOcorrencia = DateTime.UtcNow;
    }
}