using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Mappers;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class PermissaoRepository : BaseRepository<Permissao>, IPermissaoRepository
{
    private readonly IMapper<PermissaoEntity, Permissao> _mapper;

    public PermissaoRepository(GestorWorkflowDbContext context, IMapper<PermissaoEntity, Permissao> mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PermissaoEntity?> ObterPorIdAsync(int id)
    {
        var permissao = await _dbSet
            .Include(p => p.TransicaoModelo)
            .FirstOrDefaultAsync(p => p.PermissaoId == id);

        return _mapper.MapToDomain(permissao);
    }

    public async Task<IEnumerable<PermissaoEntity>> ObterTodosAsync()
    {
        var permissoes = await _dbSet
            .Include(p => p.TransicaoModelo)
            .ToListAsync();

        return permissoes.Select(p => _mapper.MapToDomain(p));
    }

    public async Task<IEnumerable<PermissaoEntity>> ObterPorTransicaoAsync(int transicaoId)
    {
        var permissoes = await _dbSet
            .Include(p => p.TransicaoModelo)
            .Where(p => p.TransicaoModeloId == transicaoId)
            .ToListAsync();

        return permissoes.Select(p => _mapper.MapToDomain(p));
    }

    public async Task<PermissaoEntity> CriarAsync(PermissaoEntity permissaoEntity)
    {
        var permissao = _mapper.MapToDataModel(permissaoEntity);
        var createdPermissao = await CreateAsync(permissao);
        return _mapper.MapToDomain(createdPermissao);
    }

    public async Task<PermissaoEntity> AtualizarAsync(PermissaoEntity permissaoEntity)
    {
        var existingPermissao = await _dbSet.FindAsync(permissaoEntity.Id);
        if (existingPermissao == null)
            throw new InvalidOperationException($"Permissao with ID {permissaoEntity.Id} not found.");

        _mapper.MapToExistingDataModel(permissaoEntity, existingPermissao);
        var updatedPermissao = Update(existingPermissao);
        return _mapper.MapToDomain(updatedPermissao);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(p => p.PermissaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(p => p.PermissaoId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var permissao = await _dbSet.FindAsync(id);
        if (permissao != null)
            Delete(permissao);
    }
}