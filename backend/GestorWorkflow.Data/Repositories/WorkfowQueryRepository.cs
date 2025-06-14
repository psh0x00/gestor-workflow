using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorWorkflow.Data.Repositories;

/// <summary>
/// Repository for querying workflow-related data and statistics
/// </summary>
public class WorkflowQueryRepository : IWorkflowQueryRepository
{
    private readonly GestorWorkflowDbContext _context;
    private readonly IMapper<WorkflowInstanciaEntity, WorkflowInstancia> _mapper;

    public WorkflowQueryRepository(
        GestorWorkflowDbContext context,
        IMapper<WorkflowInstanciaEntity, WorkflowInstancia> mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gets all active workflow instances
    /// </summary>
    public async Task<IEnumerable<WorkflowInstanciaEntity>> GetWorkflowsEmAndamentoAsync()
    {
        var workflows = await _context.WorkflowInstancias
            .Where(w => w.StatusWorkflowEntity == StatusWorkflowEntity.Ativo)
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Include(w => w.IniciadoPor)
            .AsNoTracking()
            .ToListAsync();

        return workflows.Select(_mapper.MapToDomain);
    }

    /// <summary>
    /// Gets transition statistics for a given date range
    /// </summary>
    public async Task<IEnumerable<RegistoTransicaoEntity>> GetEstatisticasTransicoesAsync(DateTime dataInicio, DateTime dataFim)
    {
        var transicoes = await _context.WorkflowInstancias
            .Include(w => w.TransicoesInstancia)
                .ThenInclude(t => t.TransicaoModelo)
            .Include(w => w.TransicoesInstancia)
                .ThenInclude(t => t.ExecutadoPor)
            .Where(w => w.TransicoesInstancia.Any(t => t.DataExecucao >= dataInicio && t.DataExecucao <= dataFim))
            .SelectMany(w => w.TransicoesInstancia)
            .Where(t => t.DataExecucao >= dataInicio && t.DataExecucao <= dataFim)
            .AsNoTracking()
            .ToListAsync();

        return transicoes.Select(t => new RegistoTransicaoEntity(
            t.TransicaoModeloId,
            t.TransicaoModelo?.EstadoOrigemId,
            t.TransicaoModelo?.EstadoDestinoId ?? 0,
            t.DataExecucao,
            t.ExecutadoPorUtilizadorId
        ));
    }

    /// <summary>
    /// Gets statistics about workflow instances by state for a given workflow model
    /// </summary>
    public async Task<Dictionary<string, int>> GetEstatisticasPorEstadoAsync(int workflowModeloId)
    {
        var instancias = await _context.WorkflowInstancias
            .Where(w => w.WorkflowModeloId == workflowModeloId)
            .Include(w => w.EstadoAtual)
            .AsNoTracking()
            .ToListAsync();

        return instancias
            .Where(w => w.EstadoAtualId.HasValue)
            .GroupBy(w => w.EstadoAtual?.Nome ?? "Desconhecido")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Gets all delayed workflow instances based on a time limit
    /// </summary>
    public async Task<IEnumerable<WorkflowInstanciaEntity>> GetWorkflowsAtrasadosAsync(TimeSpan tempoLimite)
    {
        var dataLimite = DateTime.UtcNow.Subtract(tempoLimite);

        var workflows = await _context.WorkflowInstancias
            .Where(w => w.StatusWorkflowEntity == StatusWorkflowEntity.Ativo && w.DataInicio <= dataLimite)
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Include(w => w.IniciadoPor)
            .AsNoTracking()
            .ToListAsync();

        return workflows.Select(_mapper.MapToDomain);
    }

    /// <summary>
    /// Gets the most active users in a given date range
    /// </summary>
    public async Task<IEnumerable<UtilizadorEntity>> GetUtilizadoresMaisAtivosAsync(DateTime dataInicio, DateTime dataFim, int limite = 10)
    {
        var utilizadoresAtivos = await _context.WorkflowInstancias
            .Include(w => w.TransicoesInstancia)
            .Where(w => w.TransicoesInstancia.Any(t => 
                t.DataExecucao >= dataInicio && 
                t.DataExecucao <= dataFim && 
                t.ExecutadoPorUtilizadorId.HasValue))
            .SelectMany(w => w.TransicoesInstancia)
            .Where(t => t.DataExecucao >= dataInicio && 
                       t.DataExecucao <= dataFim && 
                       t.ExecutadoPorUtilizadorId.HasValue)
            .GroupBy(t => t.ExecutadoPorUtilizadorId)
            .OrderByDescending(g => g.Count())
            .Take(limite)
            .Select(g => g.Key)
            .ToListAsync();

        var utilizadores = await _context.Utilizadores
            .Where(u => utilizadoresAtivos.Contains(u.UtilizadorId))
            .AsNoTracking()
            .ToListAsync();

        return utilizadores.Select(u => new UtilizadorEntity(
            u.UtilizadorId,
            u.Nome,
            u.Funcao
        ));
    }

    /// <summary>
    /// Gets the success rate for a given workflow model
    /// </summary>
    public async Task<decimal> GetTaxaSucessoWorkflowAsync(int workflowModeloId)
    {
        var totalInstancias = await _context.WorkflowInstancias
            .CountAsync(w => w.WorkflowModeloId == workflowModeloId);

        if (totalInstancias == 0) return 0;

        var instanciasConcluidas = await _context.WorkflowInstancias
            .CountAsync(w => w.WorkflowModeloId == workflowModeloId && 
                           w.StatusWorkflowEntity == StatusWorkflowEntity.Concluido);

        return (decimal)instanciasConcluidas / totalInstancias * 100;
    }

    /// <summary>
    /// Gets the average completion time for a given workflow model
    /// </summary>
    public async Task<TimeSpan> GetTempoMedioConclusaoAsync(int workflowModeloId)
    {
        var instanciasConcluidas = await _context.WorkflowInstancias
            .Where(w => w.WorkflowModeloId == workflowModeloId &&
                       w.StatusWorkflowEntity == StatusWorkflowEntity.Concluido &&
                       w.DataFim.HasValue)
            .Select(w => new { w.DataInicio, w.DataFim })
            .AsNoTracking()
            .ToListAsync();

        if (!instanciasConcluidas.Any()) return TimeSpan.Zero;

        var temposTotais = instanciasConcluidas
            .Select(i => i.DataFim!.Value - i.DataInicio)
            .ToList();

        var ticksMedias = (long)temposTotais.Average(t => t.Ticks);
        return new TimeSpan(ticksMedias);
    }
}