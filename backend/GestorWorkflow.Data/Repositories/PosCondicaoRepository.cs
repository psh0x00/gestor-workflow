using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorWorkflow.Data.Repositories;

public class PosCondicaoRepository : BaseRepository<PosCondicao>, IPosCondicaoRepository
{
    private readonly IMapper<PosCondicaoEntity, PosCondicao> _mapper;

    public PosCondicaoRepository(GestorWorkflowDbContext context, IMapper<PosCondicaoEntity, PosCondicao> mapper)
        : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PosCondicaoEntity?> ObterPorIdAsync(int id)
    {
        var posCondicao = await _context.PosCondicoes
            .FirstOrDefaultAsync(p => p.PosCondicaoId == id);

        return _mapper.MapToDomain(posCondicao);
    }

    public async Task<IEnumerable<PosCondicaoEntity>> ObterTodosAsync()
    {
        var posCondicoes = await _context.PosCondicoes
            .ToListAsync();

        return posCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<PosCondicaoEntity>> ObterAtivosAsync()
    {
        var posCondicoes = await _context.PosCondicoes
            .Where(p => p.Ativo)
            .ToListAsync();

        return posCondicoes.Select(_mapper.MapToDomain);
    }

    public async Task<PosCondicaoEntity> CriarAsync(PosCondicaoEntity posCondicaoEntity)
    {
        var posCondicao = _mapper.MapToDataModel(posCondicaoEntity);
        await _context.PosCondicoes.AddAsync(posCondicao);
        return _mapper.MapToDomain(posCondicao);
    }

    public async Task<PosCondicaoEntity> AtualizarAsync(PosCondicaoEntity posCondicaoEntity)
    {
        var posCondicao = await _context.PosCondicoes
            .FirstOrDefaultAsync(p => p.PosCondicaoId == posCondicaoEntity.Id);

        if (posCondicao == null)
            throw new ArgumentException("Pós-condição não encontrada", nameof(posCondicaoEntity));

        _mapper.MapToExistingDataModel(posCondicaoEntity, posCondicao);
        _context.PosCondicoes.Update(posCondicao);

        return _mapper.MapToDomain(posCondicao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.PosCondicoes
            .AnyAsync(p => p.PosCondicaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _context.PosCondicoes.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(p => p.PosCondicaoId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var posCondicao = await _context.PosCondicoes
            .FirstOrDefaultAsync(p => p.PosCondicaoId == id);

        if (posCondicao != null)
        {
            _context.PosCondicoes.Remove(posCondicao);
        }
    }
}