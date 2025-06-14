namespace GestorWorkflow.Core.Exceptions;

public class ConfiguracaoInvalidaException : WorkflowExceptionBase
{
    public ConfiguracaoInvalidaException(string configuracao, string motivo)
        : base("CONFIGURACAO_INVALIDA", $"Configuração '{configuracao}' inválida: {motivo}")
    {
    }
}