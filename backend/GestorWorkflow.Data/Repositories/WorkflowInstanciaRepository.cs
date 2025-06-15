using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories;

public class WorkflowInstanciaRepository : BaseRepository<WorkflowInstancia>, IWorkflowInstanciaRepository
{
    private readonly IMapper<WorkflowInstanciaEntity, WorkflowInstancia> _mapper;

    public WorkflowInstanciaRepository(GestorWorkflowDbContext context, IMapper<WorkflowInstanciaEntity, WorkflowInstancia> mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public async Task<WorkflowInstanciaEntity?> ObterPorIdAsync(int id)
    {
        var workflowInstancia = await _dbSet
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Include(w => w.IniciadoPor)
            .Include(w => w.TransicoesInstancia)
            .FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);

        return _mapper.MapToDomain(workflowInstancia);
    }

    public async Task<WorkflowInstanciaEntity?> ObterComHistoricoAsync(int id)
    {
        var workflowInstancia = await _dbSet
            .Include(w => w.TransicoesInstancia)
            .ThenInclude(t => t.TransicaoModelo)
            .Include(w => w.EstadoAtual)
            .FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);

        return _mapper.MapToDomainWithDetails(workflowInstancia);
    }

    public async Task<IEnumerable<WorkflowInstanciaEntity>> ObterTodosAsync()
    {
        var workflowsInstancia = await _dbSet
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .ToListAsync();

        return workflowsInstancia.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorModeloAsync(int workflowModeloId)
    {
        var workflowsInstancia = await _dbSet
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Where(w => w.WorkflowModeloId == workflowModeloId)
            .ToListAsync();

        return workflowsInstancia.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorStatusAsync(StatusWorkflowEntity status)
    {
        var workflowsInstancia = await _dbSet
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Where(w => w.StatusWorkflowEntity == status)
            .ToListAsync();

        return workflowsInstancia.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorIniciadorAsync(int iniciadorId)
    {
        var workflowsInstancia = await _dbSet
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Where(w => w.IniciadoPorUtilizadorId == iniciadorId)
            .ToListAsync();

        return workflowsInstancia.Select(_mapper.MapToDomain);
    }

    public async Task<IEnumerable<WorkflowInstanciaEntity>> ObterPorTransicaoAsync(int transicaoId)
    {
        var workflowsInstancia = await _dbSet
            .Include(w => w.TransicoesInstancia)
            .ThenInclude(ti => ti.TransicaoModelo)
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Where(w => w.TransicoesInstancia.Any(ti => ti.TransicaoModeloId == transicaoId))
            .ToListAsync();

        return workflowsInstancia.Select(_mapper.MapToDomain);
    }

    public async Task<WorkflowInstanciaEntity> CriarAsync(WorkflowInstanciaEntity workflowInstanciaEntity)
    {
        var workflowInstancia = _mapper.MapToDataModel(workflowInstanciaEntity);
        var createdWorkflowInstancia = await CreateAsync(workflowInstancia);
        return _mapper.MapToDomain(createdWorkflowInstancia);
    }

    public async Task<WorkflowInstanciaEntity> AtualizarAsync(WorkflowInstanciaEntity workflowInstanciaEntity)
    {
        var existingWorkflowInstancia = await _dbSet.FindAsync(workflowInstanciaEntity.Id);
        if (existingWorkflowInstancia == null)
            throw new InvalidOperationException($"WorkflowInstancia with ID {workflowInstanciaEntity.Id} not found.");

        _mapper.MapToExistingDataModel(workflowInstanciaEntity, existingWorkflowInstancia);
        var updatedWorkflowInstancia = Update(existingWorkflowInstancia);
        return _mapper.MapToDomain(updatedWorkflowInstancia);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _dbSet.AnyAsync(w => w.WorkflowInstanciaId == id);
    }

    public async Task RemoverAsync(int id)
    {
        var workflowInstancia = await _dbSet
            .Include(w => w.TransicoesInstancia)
            .FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);

        if (workflowInstancia != null)
            Delete(workflowInstancia);
    }
}