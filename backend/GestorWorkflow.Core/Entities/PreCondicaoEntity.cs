namespace GestorWorkflow.Core.Entities;

public class PreCondicaoEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public string? CondicaoSql { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public int? CriadoPorId { get; private set; }

    public PreCondicaoEntity(int id, string nome, int? criadoPorId = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        Id = id;
        Nome = nome;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
        CriadoPorId = criadoPorId;
    }

    public void DefinirCondicao(string condicaoSql)
    {
        CondicaoSql = condicaoSql;
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}