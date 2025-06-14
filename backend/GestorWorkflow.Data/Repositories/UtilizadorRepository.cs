using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorWorkflow.Data.Repositories;

public class UtilizadorRepository : BaseRepository<Utilizador>, IUtilizadorRepository
{
    private readonly IMapper<UtilizadorEntity, Utilizador> _mapper;

    public UtilizadorRepository(GestorWorkflowDbContext context, IMapper<UtilizadorEntity, Utilizador> mapper)
        : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UtilizadorEntity?> ObterPorIdAsync(int id)
    {
        var utilizador = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == id);

        return _mapper.MapToDomain(utilizador);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterTodosAsync()
    {
        var utilizadores = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterPorFuncaoAsync(string funcao)
    {
        var utilizadores = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .Where(u => u.Funcao == funcao)
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterComPermissaoAsync(int permissaoId)
    {
        var utilizadores = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .Where(u => u.UtilizadorPermissoes.Any(up => up.PermissaoId == permissaoId))
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<UtilizadorEntity> CriarAsync(UtilizadorEntity utilizadorEntity)
    {
        var utilizador = _mapper.MapToDataModel(utilizadorEntity);
        await _context.Utilizadores.AddAsync(utilizador);

        // Adicionar permissões
        foreach (var permissaoId in utilizadorEntity.PermissoesIds)
        {
            _context.UtilizadorPermissoes.Add(new UtilizadorPermissao
            {
                UtilizadorId = utilizador.UtilizadorId,
                PermissaoId = permissaoId
            });
        }

        return _mapper.MapToDomain(utilizador);
    }

    public async Task<UtilizadorEntity> AtualizarAsync(UtilizadorEntity utilizadorEntity)
    {
        var utilizador = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == utilizadorEntity.Id);

        if (utilizador == null)
            throw new ArgumentException("Utilizador não encontrado", nameof(utilizadorEntity));

        _mapper.MapToExistingDataModel(utilizadorEntity, utilizador);

        // Atualizar permissões
        var permissoesAtuais = utilizador.UtilizadorPermissoes.ToList();
        var permissoesParaRemover = permissoesAtuais.Where(up => !utilizadorEntity.PermissoesIds.Contains(up.PermissaoId));
        var permissoesParaAdicionar = utilizadorEntity.PermissoesIds
            .Where(id => !permissoesAtuais.Any(up => up.PermissaoId == id))
            .Select(id => new UtilizadorPermissao { UtilizadorId = utilizador.UtilizadorId, PermissaoId = id });

        _context.UtilizadorPermissoes.RemoveRange(permissoesParaRemover);
        await _context.UtilizadorPermissoes.AddRangeAsync(permissoesParaAdicionar);

        _context.Utilizadores.Update(utilizador);
        return _mapper.MapToDomain(utilizador);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Utilizadores
            .AnyAsync(u => u.UtilizadorId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _context.Utilizadores.Where(u => u.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(u => u.UtilizadorId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var utilizador = await _context.Utilizadores
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == id);

        if (utilizador != null)
        {
            _context.UtilizadorPermissoes.RemoveRange(utilizador.UtilizadorPermissoes);
            _context.Utilizadores.Remove(utilizador);
        }
    }
}