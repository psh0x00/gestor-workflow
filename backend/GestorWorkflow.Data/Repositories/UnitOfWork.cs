using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern for managing database transactions and repositories
/// </summary>
public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly GestorWorkflowDbContext _context;
    private readonly IMapper<EstadoEntity, EstadoModelo> _estadoMapper;
    private readonly IMapper<TransicaoEntity, TransicaoModelo> _transicaoMapper;
    private readonly IMapper<WorkflowModeloEntity, WorkflowModelo> _workflowModeloMapper;
    private readonly IMapper<WorkflowInstanciaEntity, WorkflowInstancia> _workflowInstanciaMapper;
    private readonly IMapper<UtilizadorEntity, Utilizador> _utilizadorMapper;
    private readonly IMapper<PermissaoEntity, Permissao> _permissaoMapper;
    private readonly IMapper<PreCondicaoEntity, PreCondicao> _preCondicaoMapper;
    private readonly IMapper<PosCondicaoEntity, PosCondicao> _posCondicaoMapper;
    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;
    private bool _disposed;

    // Lazy initialization dos repositories
    private IEstadoRepository? _estados;
    private ITransicaoRepository? _transicoes;
    private IWorkflowModeloRepository? _workflowModelos;
    private IWorkflowInstanciaRepository? _workflowInstancias;
    private IUtilizadorRepository? _utilizadores;
    private IPermissaoRepository? _permissoes;
    private IPreCondicaoRepository? _preCondicoes;
    private IPosCondicaoRepository? _posCondicoes;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="estadoMapper">Mapper for Estado entities</param>
    /// <param name="transicaoMapper">Mapper for Transicao entities</param>
    /// <param name="workflowModeloMapper">Mapper for WorkflowModelo entities</param>
    /// <param name="workflowInstanciaMapper">Mapper for WorkflowInstancia entities</param>
    /// <param name="utilizadorMapper">Mapper for Utilizador entities</param>
    /// <param name="permissaoMapper">Mapper for Permissao entities</param>
    /// <param name="preCondicaoMapper">Mapper for PreCondicao entities</param>
    /// <param name="posCondicaoMapper">Mapper for PosCondicao entities</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
    public UnitOfWork(
        GestorWorkflowDbContext context,
        IMapper<EstadoEntity, EstadoModelo> estadoMapper,
        IMapper<TransicaoEntity, TransicaoModelo> transicaoMapper,
        IMapper<WorkflowModeloEntity, WorkflowModelo> workflowModeloMapper,
        IMapper<WorkflowInstanciaEntity, WorkflowInstancia> workflowInstanciaMapper,
        IMapper<UtilizadorEntity, Utilizador> utilizadorMapper,
        IMapper<PermissaoEntity, Permissao> permissaoMapper,
        IMapper<PreCondicaoEntity, PreCondicao> preCondicaoMapper,
        IMapper<PosCondicaoEntity, PosCondicao> posCondicaoMapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _estadoMapper = estadoMapper ?? throw new ArgumentNullException(nameof(estadoMapper));
        _transicaoMapper = transicaoMapper ?? throw new ArgumentNullException(nameof(transicaoMapper));
        _workflowModeloMapper = workflowModeloMapper ?? throw new ArgumentNullException(nameof(workflowModeloMapper));
        _workflowInstanciaMapper = workflowInstanciaMapper ?? throw new ArgumentNullException(nameof(workflowInstanciaMapper));
        _utilizadorMapper = utilizadorMapper ?? throw new ArgumentNullException(nameof(utilizadorMapper));
        _permissaoMapper = permissaoMapper ?? throw new ArgumentNullException(nameof(permissaoMapper));
        _preCondicaoMapper = preCondicaoMapper ?? throw new ArgumentNullException(nameof(preCondicaoMapper));
        _posCondicaoMapper = posCondicaoMapper ?? throw new ArgumentNullException(nameof(posCondicaoMapper));
    }

    /// <summary>
    /// Gets the Estados repository
    /// </summary>
    public IEstadoRepository Estados =>
        _estados ??= new EstadoRepository(_context, _estadoMapper);

    /// <summary>
    /// Gets the Transicoes repository
    /// </summary>
    public ITransicaoRepository Transicoes =>
        _transicoes ??= new TransicaoRepository(_context, _transicaoMapper);

    /// <summary>
    /// Gets the WorkflowModelos repository
    /// </summary>
    public IWorkflowModeloRepository WorkflowModelos =>
        _workflowModelos ??= new WorkflowModeloRepository(_context, _workflowModeloMapper);

    /// <summary>
    /// Gets the WorkflowInstancias repository
    /// </summary>
    public IWorkflowInstanciaRepository WorkflowInstancias =>
        _workflowInstancias ??= new WorkflowInstanciaRepository(_context, _workflowInstanciaMapper);

    /// <summary>
    /// Gets the Utilizadores repository
    /// </summary>
    public IUtilizadorRepository Utilizadores =>
        _utilizadores ??= new UtilizadorRepository(_context, _utilizadorMapper);

    /// <summary>
    /// Gets the Permissoes repository
    /// </summary>
    public IPermissaoRepository Permissoes =>
        _permissoes ??= new PermissaoRepository(_context, _permissaoMapper);

    /// <summary>
    /// Gets the PreCondicoes repository
    /// </summary>
    public IPreCondicaoRepository PreCondicoes =>
        _preCondicoes ??= new PreCondicaoRepository(_context, _preCondicaoMapper);

    /// <summary>
    /// Gets the PosCondicoes repository
    /// </summary>
    public IPosCondicaoRepository PosCondicoes =>
        _posCondicoes ??= new PosCondicaoRepository(_context, _posCondicaoMapper);

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error saving changes to the database", ex);
        }
    }

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when a transaction is already active</exception>
    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Uma transação já está ativa.");
        }

        try
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error beginning transaction", ex);
        }
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active</exception>
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Nenhuma transação ativa para confirmar.");
        }

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active</exception>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Nenhuma transação ativa para reverter.");
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the unit of work and its resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work and its resources asynchronously
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work and its resources
    /// </summary>
    /// <param name="disposing">True if called from Dispose, false if called from finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Disposes the unit of work and its resources asynchronously
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
            }

            if (_context != null)
            {
                await _context.DisposeAsync().ConfigureAwait(false);
            }

            _disposed = true;
        }
    }
}