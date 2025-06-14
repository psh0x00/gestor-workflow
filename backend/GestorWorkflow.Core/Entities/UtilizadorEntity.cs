namespace GestorWorkflow.Core.Entities;

public class UtilizadorEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Funcao { get; private set; }
    public List<int> PermissoesIds { get; private set; }

    public UtilizadorEntity(int id, string nome, string funcao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));
        if (string.IsNullOrWhiteSpace(funcao))
            throw new ArgumentException("Função é obrigatória", nameof(funcao));

        Id = id;
        Nome = nome;
        Funcao = funcao;
        PermissoesIds = new List<int>();
    }

    public void AdicionarPermissao(int permissaoId)
    {
        if (!PermissoesIds.Contains(permissaoId))
            PermissoesIds.Add(permissaoId);
    }

    public void RemoverPermissao(int permissaoId)
    {
        PermissoesIds.Remove(permissaoId);
    }

    public bool TemPermissao(int permissaoId)
    {
        return PermissoesIds.Contains(permissaoId);
    }
}