using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorWorkflow.Data.Repositories;

public class PreCondicaoRepository : BaseRepository<PreCondicao>, IPreCondicaoRepository
{
    private readonly IMapper<PreCondicaoEntity, PreCondicao> _mapper;

    public PreCondicaoRepository(GestorWorkflowDbContext context, IMapper<PreCondicaoEntity, PreCondicao> mapper) 
        : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PreCondicaoEntity?> ObterPorIdAsync(int id)
    {
        var preCondicao = await _context.PreCondicoes
            .FirstOrDefaultAsync(p => p.PreCondicaoId == id);

        return _mapper.MapToDomain(preCondicao);
    }

    public async Task<IEnumerable<PreCondicaoEntity>> ObterTodosAsync()
    {
        var preCondicoes = await _context.PreCondicoes
            .ToListAsync();

        return preCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<PreCondicaoEntity>> ObterAtivosAsync()
    {
        var preCondicoes = await _context.PreCondicoes
            .Where(p => p.Ativo)
            .ToListAsync();

        return preCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<PreCondicaoEntity> CriarAsync(PreCondicaoEntity preCondicaoEntity)
    {
        var preCondicao = _mapper.MapToDataModel(preCondicaoEntity);
        await _context.PreCondicoes.AddAsync(preCondicao);
        return _mapper.MapToDomain(preCondicao);
    }

    public async Task<PreCondicaoEntity> AtualizarAsync(PreCondicaoEntity preCondicaoEntity)
    {
        var preCondicao = await _context.PreCondicoes
            .FirstOrDefaultAsync(p => p.PreCondicaoId == preCondicaoEntity.Id);

        if (preCondicao == null)
            throw new ArgumentException("Pré-condição não encontrada", nameof(preCondicaoEntity));

        _mapper.MapToExistingDataModel(preCondicaoEntity, preCondicao);
        _context.PreCondicoes.Update(preCondicao);

        return _mapper.MapToDomain(preCondicao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.PreCondicoes
            .AnyAsync(p => p.PreCondicaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _context.PreCondicoes.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(p => p.PreCondicaoId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var preCondicao = await _context.PreCondicoes
            .FirstOrDefaultAsync(p => p.PreCondicaoId == id);

        if (preCondicao != null)
        {
            _context.PreCondicoes.Remove(preCondicao);
        }
    }
}