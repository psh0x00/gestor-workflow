namespace GestorWorkflow.Core.Entities;

public class TransicaoEntity
{
    public int Id { get; private set; }
    public string? Nome { get; private set; }
    public string? Descricao { get; private set; }
    public int? EstadoOrigemId { get; private set; }
    public int EstadoDestinoId { get; private set; }
    public int? PreCondicaoId { get; private set; }
    public int? PosCondicaoId { get; private set; }
    public List<int> PermissoesIds { get; private set; }

    public TransicaoEntity(int id, int estadoDestinoId, int? estadoOrigemId = null)
    {
        Id = id;
        EstadoDestinoId = estadoDestinoId;
        EstadoOrigemId = estadoOrigemId;
        PermissoesIds = new List<int>();
    }

    public void DefinirNome(string nome)
    {
        Nome = nome;
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = descricao;
    }

    public void DefinirPreCondicao(int? preCondicaoId)
    {
        PreCondicaoId = preCondicaoId;
    }

    public void DefinirPosCondicao(int? posCondicaoId)
    {
        PosCondicaoId = posCondicaoId;
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

    public bool EhTransicaoInicial() => EstadoOrigemId == null;
}