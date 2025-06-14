namespace GestorWorkflow.Core.Entities;

public class RegistoTransicaoEntity
{
    public int TransicaoId { get; private set; }
    public int? EstadoOrigemId { get; private set; }
    public int EstadoDestinoId { get; private set; }
    public DateTime DataExecucao { get; private set; }
    public int? ExecutadoPorId { get; private set; }
    public bool Sucesso { get; private set; }
    public string? MensagemErro { get; private set; }

    public RegistoTransicaoEntity(int transicaoId, int? estadoOrigemId, int estadoDestinoId,
        DateTime dataExecucao, int? executadoPorId = null)
    {
        TransicaoId = transicaoId;
        EstadoOrigemId = estadoOrigemId;
        EstadoDestinoId = estadoDestinoId;
        DataExecucao = dataExecucao;
        ExecutadoPorId = executadoPorId;
        Sucesso = true;
    }

    public void MarcarComoFalha(string mensagemErro)
    {
        Sucesso = false;
        MensagemErro = mensagemErro;
    }
}