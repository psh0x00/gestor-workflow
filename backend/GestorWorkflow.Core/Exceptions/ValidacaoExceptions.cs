namespace GestorWorkflow.Core.Exceptions;

public class ValidacaoWorkflowException : WorkflowExceptionBase
{
    public List<string> ErrosValidacao { get; }

    public ValidacaoWorkflowException(List<string> erros)
        : base("VALIDACAO_WORKFLOW_FALHOU", $"Validação falhou com {erros.Count} erro(s).")
    {
        ErrosValidacao = erros;
    }

    public ValidacaoWorkflowException(string erro)
        : base("VALIDACAO_WORKFLOW_FALHOU", erro)
    {
        ErrosValidacao = new List<string> { erro };
    }
}

public class DadosInvalidosException : WorkflowExceptionBase
{
    public DadosInvalidosException(string campo, string motivo)
        : base("DADOS_INVALIDOS", $"Campo '{campo}' inválido: {motivo}")
    {
    }
}