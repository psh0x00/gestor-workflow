using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class PreCondicaoRepository : BaseRepository<PreCondicao>, IPreCondicaoRepository
{
    private readonly IMapper<PreCondicaoEntity, PreCondicao> _mapper;

    public PreCondicaoRepository(GestorWorkflowDbContext context, IMapper<PreCondicaoEntity, PreCondicao> mapper) 
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PreCondicaoEntity?> ObterPorIdAsync(int id)
    {
        var preCondicao = await _dbSet
            .Include(p => p.CriadoPor)
            .FirstOrDefaultAsync(p => p.PreCondicaoId == id);

        return _mapper.MapToDomain(preCondicao);
    }

    public async Task<IEnumerable<PreCondicaoEntity>> ObterTodosAsync()
    {
        var preCondicoes = await _dbSet
            .Include(p => p.CriadoPor)
            .ToListAsync();

        return preCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<PreCondicaoEntity>> ObterAtivosAsync()
    {
        var preCondicoes = await _dbSet
            .Include(p => p.CriadoPor)
            .Where(p => p.Ativo)
            .ToListAsync();

        return preCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<PreCondicaoEntity> CriarAsync(PreCondicaoEntity preCondicaoEntity)
    {
        var preCondicao = _mapper.MapToDataModel(preCondicaoEntity);
        var createdPreCondicao = await CreateAsync(preCondicao);
        return _mapper.MapToDomain(createdPreCondicao);
    }

    public async Task<PreCondicaoEntity> AtualizarAsync(PreCondicaoEntity preCondicaoEntity)
    {
        var existingPreCondicao = await _dbSet.FindAsync(preCondicaoEntity.Id);
        if (existingPreCondicao == null)
            throw new InvalidOperationException($"PreCondicao with ID {preCondicaoEntity.Id} not found.");

        _mapper.MapToExistingDataModel(preCondicaoEntity, existingPreCondicao);
        var updatedPreCondicao = Update(existingPreCondicao);
        return _mapper.MapToDomain(updatedPreCondicao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(p => p.PreCondicaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(p => p.PreCondicaoId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var preCondicao = await _dbSet.FindAsync(id);
        if (preCondicao != null)
            Delete(preCondicao);
    }
}