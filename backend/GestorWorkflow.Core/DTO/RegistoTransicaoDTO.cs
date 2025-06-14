namespace GestorWorkflow.Core.DTO;

public class RegistoTransicaoDTO
{
    public int TransicaoId { get; set; }
    public int? EstadoOrigemId { get; set; }
    public int EstadoDestinoId { get; set; }
    public DateTime DataExecucao { get; set; }
    public int? ExecutadoPorId { get; set; }
    public bool Sucesso { get; set; }
    public string? MensagemErro { get; set; }

    // Propriedades adicionais para melhor visualização
    public string? NomeTransicao { get; set; }
    public string? NomeEstadoOrigem { get; set; }
    public string? NomeEstadoDestino { get; set; }
    public string? NomeExecutor { get; set; }
}

public class ExecutarTransicaoDTO
{
    public int TransicaoId { get; set; }
    public int WorkflowInstanciaId { get; set; }
    public int? ExecutadoPorId { get; set; }
}