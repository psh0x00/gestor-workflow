namespace GestorWorkflow.Core.Interfaces;

public interface IWorkflowException
{
    string Codigo { get; }
    string Detalhes { get; }
    DateTime DataOcorrencia { get; }
}