using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class UtilizadorRepository : BaseRepository<Utilizador>, IUtilizadorRepository
{
    private readonly IMapper<UtilizadorEntity, Utilizador> _mapper;

    public UtilizadorRepository(GestorWorkflowDbContext context, IMapper<UtilizadorEntity, Utilizador> mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<UtilizadorEntity?> ObterPorIdAsync(int id)
    {
        var utilizador = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == id);

        return _mapper.MapToDomain(utilizador);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterTodosAsync()
    {
        var utilizadores = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterPorFuncaoAsync(string funcao)
    {
        var utilizadores = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .Where(u => u.Funcao == funcao)
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<UtilizadorEntity>> ObterComPermissaoAsync(int permissaoId)
    {
        var utilizadores = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .Where(u => u.UtilizadorPermissoes.Any(up => up.PermissaoId == permissaoId))
            .ToListAsync();

        return utilizadores.Select(_mapper.MapToDomain);
    }

    public async Task<UtilizadorEntity> CriarAsync(UtilizadorEntity utilizadorEntity)
    {
        var utilizador = _mapper.MapToDataModel(utilizadorEntity);
        var createdUtilizador = await CreateAsync(utilizador);

        // Adicionar permissões
        foreach (var permissaoId in utilizadorEntity.PermissoesIds)
        {
            _context.UtilizadorPermissoes.Add(new UtilizadorPermissao
            {
                UtilizadorId = createdUtilizador.UtilizadorId,
                PermissaoId = permissaoId
            });
        }

        await _context.SaveChangesAsync();
        return _mapper.MapToDomain(createdUtilizador);
    }

    public async Task<UtilizadorEntity> AtualizarAsync(UtilizadorEntity utilizadorEntity)
    {
        var existingUtilizador = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == utilizadorEntity.Id);

        if (existingUtilizador == null)
            throw new InvalidOperationException($"Utilizador with ID {utilizadorEntity.Id} not found.");

        _mapper.MapToExistingDataModel(utilizadorEntity, existingUtilizador);

        // Atualizar permissões
        var permissoesAtuais = existingUtilizador.UtilizadorPermissoes.ToList();
        var permissoesParaRemover = permissoesAtuais.Where(up => !utilizadorEntity.PermissoesIds.Contains(up.PermissaoId));
        var permissoesParaAdicionar = utilizadorEntity.PermissoesIds
            .Where(id => !permissoesAtuais.Any(up => up.PermissaoId == id))
            .Select(id => new UtilizadorPermissao { UtilizadorId = existingUtilizador.UtilizadorId, PermissaoId = id });

        _context.UtilizadorPermissoes.RemoveRange(permissoesParaRemover);
        await _context.UtilizadorPermissoes.AddRangeAsync(permissoesParaAdicionar);

        var updatedUtilizador = Update(existingUtilizador);
        await _context.SaveChangesAsync();
        return _mapper.MapToDomain(updatedUtilizador);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(u => u.UtilizadorId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(u => u.UtilizadorId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var utilizador = await _dbSet
            .Include(u => u.UtilizadorPermissoes)
            .FirstOrDefaultAsync(u => u.UtilizadorId == id);

        if (utilizador != null)
        {
            _context.UtilizadorPermissoes.RemoveRange(utilizador.UtilizadorPermissoes);
            Delete(utilizador);
            await _context.SaveChangesAsync();
        }
    }
}