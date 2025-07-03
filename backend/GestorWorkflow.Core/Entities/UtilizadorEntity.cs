namespace GestorWorkflow.Core.Entities;

public class UtilizadorEntity
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Funcao { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public List<int> PermissoesIds { get; private set; }

    public UtilizadorEntity(int id, string nome, string funcao, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));
        // Permitir funcao nulo ou vazio, apenas warning opcional
        if (funcao == null)
            funcao = string.Empty;
        //if (string.IsNullOrWhiteSpace(funcao))
        //    throw new ArgumentException("Função é obrigatória", nameof(funcao));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash é obrigatório", nameof(passwordHash));

        Id = id;
        Nome = nome;
        Funcao = funcao;
        Email = email;
        PasswordHash = passwordHash;
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