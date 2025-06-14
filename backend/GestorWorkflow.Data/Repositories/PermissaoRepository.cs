using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Mappers;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorWorkflow.Data.Repositories;

public class PermissaoRepository : BaseRepository<Permissao>, IPermissaoRepository
{
    private readonly IMapper<PermissaoEntity, Permissao> _mapper;

    public PermissaoRepository(GestorWorkflowDbContext context, IMapper<PermissaoEntity, Permissao> mapper)
        : base(context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PermissaoEntity?> ObterPorIdAsync(int id)
    {
        var permissao = await _context.Permissoes
            .FirstOrDefaultAsync(p => p.PermissaoId == id);

        return _mapper.MapToDomain(permissao);
    }

    public async Task<IEnumerable<PermissaoEntity>> ObterTodosAsync()
    {
        var permissoes = await _context.Permissoes
            .ToListAsync();

        return permissoes.Select(p => _mapper.MapToDomain(p));
    }

    public async Task<IEnumerable<PermissaoEntity>> ObterPorTransicaoAsync(int transicaoId)
    {
        var permissoes = await _context.Permissoes
            .Where(p => p.TransicaoModeloId == transicaoId)
            .ToListAsync();

        return permissoes.Select(p => _mapper.MapToDomain(p));
    }

    public async Task<PermissaoEntity> CriarAsync(PermissaoEntity permissaoEntity)
    {
        var permissao = _mapper.MapToDataModel(permissaoEntity);
        await _context.Permissoes.AddAsync(permissao);
        await _context.SaveChangesAsync();

        return _mapper.MapToDomain(permissao);
    }

    public async Task<PermissaoEntity> AtualizarAsync(PermissaoEntity permissaoEntity)
    {
        var permissao = await _context.Permissoes
            .FirstOrDefaultAsync(p => p.PermissaoId == permissaoEntity.Id);

        if (permissao == null)
            throw new ArgumentException("Permissão não encontrada", nameof(permissaoEntity));

        _mapper.MapToExistingDataModel(permissaoEntity, permissao);
        _context.Permissoes.Update(permissao);
        await _context.SaveChangesAsync();

        return _mapper.MapToDomain(permissao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Permissoes
            .AnyAsync(p => p.PermissaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _context.Permissoes.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.PermissaoId != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var permissao = await _context.Permissoes
            .FirstOrDefaultAsync(p => p.PermissaoId == id);

        if (permissao != null)
        {
            _context.Permissoes.Remove(permissao);
            await _context.SaveChangesAsync();
        }
    }
}