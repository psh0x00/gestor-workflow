using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class PosCondicaoRepository : BaseRepository<PosCondicao>, IPosCondicaoRepository
{
    private readonly IMapper<PosCondicaoEntity, PosCondicao> _mapper;

    public PosCondicaoRepository(GestorWorkflowDbContext context, IMapper<PosCondicaoEntity, PosCondicao> mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PosCondicaoEntity?> ObterPorIdAsync(int id)
    {
        var posCondicao = await _dbSet
            .Include(p => p.CriadoPor)
            .FirstOrDefaultAsync(p => p.PosCondicaoId == id);

        return _mapper.MapToDomain(posCondicao);
    }

    public async Task<IEnumerable<PosCondicaoEntity>> ObterTodosAsync()
    {
        var posCondicoes = await _dbSet
            .Include(p => p.CriadoPor)
            .ToListAsync();

        return posCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<PosCondicaoEntity>> ObterAtivosAsync()
    {
        var posCondicoes = await _dbSet
            .Include(p => p.CriadoPor)
            .Where(p => p.Ativo)
            .ToListAsync();

        return posCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<PosCondicaoEntity> CriarAsync(PosCondicaoEntity posCondicaoEntity)
    {
        var posCondicao = _mapper.MapToDataModel(posCondicaoEntity);
        var createdPosCondicao = await CreateAsync(posCondicao);
        return _mapper.MapToDomain(createdPosCondicao);
    }

    public async Task<PosCondicaoEntity> AtualizarAsync(PosCondicaoEntity posCondicaoEntity)
    {
        var existingPosCondicao = await _dbSet.FindAsync(posCondicaoEntity.Id);
        if (existingPosCondicao == null)
            throw new InvalidOperationException($"PosCondicao with ID {posCondicaoEntity.Id} not found.");

        _mapper.MapToExistingDataModel(posCondicaoEntity, existingPosCondicao);
        var updatedPosCondicao = Update(existingPosCondicao);
        return _mapper.MapToDomain(updatedPosCondicao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(p => p.PosCondicaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(p => p.PosCondicaoId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var posCondicao = await _dbSet.FindAsync(id);
        if (posCondicao != null)
            Delete(posCondicao);
    }
}