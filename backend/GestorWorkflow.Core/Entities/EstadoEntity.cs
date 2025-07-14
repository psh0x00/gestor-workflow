using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.Entities;

public class EstadoEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public TipoEstadoEntity Tipo { get; private set; }
    public string? CorHexadecimal { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public int? CriadoPorId { get; private set; }

    public List<string> Funcoes { get; private set; } = new();

    // Adiciona propriedades explícitas para condições
    public string? PreCondicao { get; private set; }
    public string? PosCondicao { get; private set; }

    public void DefinirPreCondicao(string? preCondicao)
    {
        PreCondicao = preCondicao;
    }

    public void DefinirPosCondicao(string? posCondicao)
    {
        PosCondicao = posCondicao;
    }

    public EstadoEntity(int id, string nome, TipoEstadoEntity tipo, int? criadoPorId = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        Id = id;
        Nome = nome;
        Tipo = tipo;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
        CriadoPorId = criadoPorId;
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao;
    }

    public void DefinirCor(string? corHexadecimal)
    {
        if (!string.IsNullOrEmpty(corHexadecimal) && !corHexadecimal.StartsWith("#"))
            throw new ArgumentException("Cor deve começar com #", nameof(corHexadecimal));

        CorHexadecimal = corHexadecimal;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void DefinirFuncoes(List<string> funcoes)
    {
        Funcoes = funcoes ?? new List<string>();
    }

    public bool EhEstadoInicial() => Tipo == TipoEstadoEntity.Inicial;
    public bool EhEstadoFinal() => Tipo == TipoEstadoEntity.Final;
}