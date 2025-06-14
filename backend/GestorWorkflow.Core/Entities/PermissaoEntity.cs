namespace GestorWorkflow.Core.Entities;

public class PermissaoEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public int TransicaoId { get; private set; }

    public PermissaoEntity(int id, string nome, int transicaoId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        Id = id;
        Nome = nome;
        TransicaoId = transicaoId;
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao;
    }
}