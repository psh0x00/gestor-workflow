using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class WorkflowModeloRepository : BaseRepository<WorkflowModelo>, IWorkflowModeloRepository
{
    private readonly IMapper<WorkflowModeloEntity, WorkflowModelo> _mapper;

    public WorkflowModeloRepository(GestorWorkflowDbContext context, IMapper<WorkflowModeloEntity, WorkflowModelo> mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<WorkflowModeloEntity?> ObterPorIdAsync(int id)
    {
        var workflowModelo = await _dbSet
            .Include(w => w.EstadoInicial)
            .Include(w => w.CriadoPor)
            .Include(w => w.AlteradoPor)
            .FirstOrDefaultAsync(w => w.WorkflowModeloId == id);

        return _mapper.MapToDomain(workflowModelo);
    }

    public async Task<WorkflowModeloEntity?> ObterComDetalhesAsync(int id)
    {
        var workflowModelo = await _dbSet
            .Include(w => w.EstadoInicial)
            .Include(w => w.TransicoesModelo)
                .ThenInclude(t => t.EstadoOrigem)
            .Include(w => w.TransicoesModelo)
                .ThenInclude(t => t.EstadoDestino)
            .Include(w => w.TransicoesModelo)
                .ThenInclude(t => t.PreCondicao)
            .Include(w => w.TransicoesModelo)
                .ThenInclude(t => t.PosCondicao)
            .Include(w => w.TransicoesModelo)
                .ThenInclude(t => t.Permissoes)
            .Include(w => w.CriadoPor)
            .Include(w => w.AlteradoPor)
            .FirstOrDefaultAsync(w => w.WorkflowModeloId == id);

        return _mapper.MapToDomainWithDetails(workflowModelo);
    }

    public async Task<IEnumerable<WorkflowModeloEntity>> ObterTodosAsync()
    {
        var workflowModelos = await _dbSet
            .Include(w => w.EstadoInicial)
            .Include(w => w.CriadoPor)
            .ToListAsync();

        return workflowModelos.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowModeloEntity>> ObterAtivosAsync()
    {
        var workflowModelos = await _dbSet
            .Include(w => w.EstadoInicial)
            .Include(w => w.CriadoPor)
            .Where(w => w.Ativo)
            .ToListAsync();

        return workflowModelos.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowModeloEntity>> ObterPorCriadorAsync(int criadorId)
    {
        var workflowModelos = await _dbSet
            .Include(w => w.EstadoInicial)
            .Include(w => w.CriadoPor)
            .Where(w => w.CriadoPorUtilizadorId == criadorId)
            .ToListAsync();

        return workflowModelos.Select(_mapper.MapToDomain);
    }

    public async Task<WorkflowModeloEntity> CriarAsync(WorkflowModeloEntity workflowModeloEntity)
    {
        var workflowModelo = _mapper.MapToDataModel(workflowModeloEntity);
        var createdWorkflowModelo = await CreateAsync(workflowModelo);
        return _mapper.MapToDomain(createdWorkflowModelo);
    }

    public async Task<WorkflowModeloEntity> AtualizarAsync(WorkflowModeloEntity workflowModeloEntity)
    {
        var existingWorkflowModelo = await _dbSet.FindAsync(workflowModeloEntity.Id);
        if (existingWorkflowModelo == null)
            throw new InvalidOperationException($"WorkflowModelo with ID {workflowModeloEntity.Id} not found.");

        _mapper.MapToExistingDataModel(workflowModeloEntity, existingWorkflowModelo);
        var updatedWorkflowModelo = Update(existingWorkflowModelo);
        return _mapper.MapToDomain(updatedWorkflowModelo);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(w => w.WorkflowModeloId == id);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
    {
        var query = _dbSet.Where(w => w.Nome == nome);

        if (excludeId.HasValue)
            query = query.Where(w => w.WorkflowModeloId != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var workflowModelo = await _dbSet
            .Include(w => w.WorkflowInstancias)
            .FirstOrDefaultAsync(w => w.WorkflowModeloId == id);

        if (workflowModelo != null)
        {
            if (workflowModelo.WorkflowInstancias.Any())
            {
                throw new InvalidOperationException("Não é possível remover o workflow modelo pois existem instâncias associadas.");
            }

            Delete(workflowModelo);
        }
    }
}