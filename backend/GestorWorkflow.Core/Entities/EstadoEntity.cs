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

    public bool EhEstadoInicial() => Tipo == TipoEstadoEntity.Inicial;
    public bool EhEstadoFinal() => Tipo == TipoEstadoEntity.Final;
}