using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using static GestorWorkflow.Core.Services.ServicesMappings;

namespace GestorWorkflow.Core.Services;

public class WorkflowModeloService : IWorkflowModeloService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkflowModeloService> _logger;

    public WorkflowModeloService(IUnitOfWork unitOfWork, ILogger<WorkflowModeloService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WorkflowModeloDTO> CriarWorkflowModeloAsync(CriarWorkflowModeloDTO dto)
    {
        _logger.LogInformation("Criando novo workflow modelo: {Nome}", dto.Nome);

        if (await _unitOfWork.WorkflowModelos.ExisteNomeAsync(dto.Nome))
            throw new WorkflowModeloNomeJaExisteException(dto.Nome);

        var novoId = await GerarProximoIdWorkflowModeloAsync();

        // 1. Criar estados e guardar os IDs
        var estadoIdMap = new Dictionary<int, int>(); // idx original -> id criado
        int? estadoInicialId = null;
        var estadosCriados = new List<EstadoEntity>();

        for (int i = 0; i < dto.Estados.Count; i++)
        {
            var estadoDto = dto.Estados[i];
            var estadoEntity = new EstadoEntity(
                0, // o ID será gerado pela BD
                estadoDto.Nome,
                estadoDto.Tipo,
                estadoDto.CriadoPorId
            );
            estadoEntity.AtualizarDescricao(estadoDto.Descricao);
            estadoEntity.DefinirCor(estadoDto.CorHexadecimal);

            var estadoCriado = await _unitOfWork.Estados.CriarAsync(estadoEntity);
            await _unitOfWork.SaveChangesAsync();
            estadoIdMap[i] = estadoCriado.Id;
            estadosCriados.Add(estadoCriado);

            if (estadoDto.IsInicial)
                estadoInicialId = estadoCriado.Id;
        }

        if (estadoInicialId == null)
            throw new Exception("Nenhum estado inicial definido.");

        // 2. Criar o modelo de workflow com o estado inicial correto
        var workflow = new WorkflowModeloEntity(novoId, dto.Nome, estadoInicialId.Value, dto.CriadoPorId);
        if (!string.IsNullOrEmpty(dto.Descricao))
            workflow.AtualizarDescricao(dto.Descricao);

        // Associar estados ao modelo
        foreach (var estado in estadosCriados)
            workflow.AdicionarEstado(estado);

        var workflowCriado = await _unitOfWork.WorkflowModelos.CriarAsync(workflow);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Workflow modelo criado com sucesso: ID {Id}", workflowCriado.Id);

        return await MapearParaWorkflowModeloDtoAsync(workflowCriado);
    }

    public async Task<WorkflowModeloDTO> ObterWorkflowModeloPorIdAsync(int id)
    {
        var workflow = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(id);
        if (workflow == null)
            throw new WorkflowModeloNaoEncontradoException(id);

        return await MapearParaWorkflowModeloDtoAsync(workflow);
    }

    public async Task<IEnumerable<WorkflowModeloDTO>> ObterTodosWorkflowModelosAsync()
    {
        var workflows = await _unitOfWork.WorkflowModelos.ObterTodosAsync();
        var workflowsDTOs = new List<WorkflowModeloDTO>();

        foreach (var workflow in workflows)
            workflowsDTOs.Add(await MapearParaWorkflowModeloDtoAsync(workflow));

        return workflowsDTOs;
    }

    public async Task<WorkflowModeloDTO> AtualizarWorkflowModeloAsync(int id, AtualizarWorkflowModeloDTO dto)
    {
        try
        {
            var workflow = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(id);
            if (workflow == null)
                throw new WorkflowModeloNaoEncontradoException(id);

            // Verificar se o novo nome já existe em outro workflow
            if (!string.IsNullOrEmpty(dto.Nome) && dto.Nome != workflow.Nome && await _unitOfWork.WorkflowModelos.ExisteNomeAsync(dto.Nome))
                throw new WorkflowModeloNomeJaExisteException(dto.Nome!);

            if (!string.IsNullOrEmpty(dto.Nome))
                workflow.AtualizarNome(dto.Nome!);
            if (!string.IsNullOrEmpty(dto.Descricao))
                workflow.AtualizarDescricao(dto.Descricao);

            var workflowAtualizado = await _unitOfWork.WorkflowModelos.AtualizarAsync(workflow);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaWorkflowModeloDtoAsync(workflowAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar workflow modelo: ID {Id}", id);
            throw;
        }
    }

    public async Task RemoverWorkflowModeloAsync(int id)
    {
        try
        {
            if (!await _unitOfWork.WorkflowModelos.ExisteAsync(id))
                throw new WorkflowModeloNaoEncontradoException(id);

            // Verificar se existem instâncias ativas
            var instancias = await _unitOfWork.WorkflowInstancias.ObterPorModeloAsync(id);
            if (instancias.Any(i => i.Status == StatusWorkflowEntity.Ativo))
                throw new RecursoEmUsoException("Workflow Modelo", id);

            await _unitOfWork.WorkflowModelos.RemoverAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Workflow modelo removido com sucesso: ID {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover workflow modelo: ID {Id}", id);
            throw;
        }
    }

    private async Task<int> GerarProximoIdWorkflowModeloAsync()
    {
        var ultimoWorkflow = (await _unitOfWork.WorkflowModelos.ObterTodosAsync())
            .OrderByDescending(w => w.Id)
            .FirstOrDefault();
        return (ultimoWorkflow?.Id ?? 0) + 1;
    }

    private async Task<WorkflowModeloDTO> MapearParaWorkflowModeloDtoAsync(WorkflowModeloEntity workflow)
    {

        var dto = new WorkflowModeloDTO
        {
            Id = workflow.Id,
            Nome = workflow.Nome,
            Descricao = workflow.Descricao,
            Versao = workflow.Versao,
            EstadoInicialId = workflow.EstadoInicialId,
            Ativo = workflow.Ativo,
            DataCriacao = workflow.DataCriacao,
            DataUltimaAlteracao = workflow.DataUltimaAlteracao,
            CriadoPorId = workflow.CriadoPorId,
            AlteradoPorId = workflow.AlteradoPorId
        };

        // Mapear estados
        foreach (var estado in workflow.Estados) dto.Estados.Add(MapearParaEstadoDto(estado));

        // Mapear transições
        foreach (var transicao in workflow.Transicoes) dto.Transicoes.Add(await MapearParaTransicaoDtoAsync(transicao, _unitOfWork));

        // Adicionar informações adicionais
        var estadoInicial = await _unitOfWork.Estados.ObterPorIdAsync(workflow.EstadoInicialId);
        dto.NomeEstadoInicial = estadoInicial?.Nome;

        if (workflow.CriadoPorId > 0)
        {
            var criador = await _unitOfWork.Utilizadores.ObterPorIdAsync(workflow.CriadoPorId);
            dto.NomeCriador = criador?.Nome;
        }

        if (workflow.AlteradoPorId.HasValue && workflow.AlteradoPorId > 0)
        {
            var alterador = await _unitOfWork.Utilizadores.ObterPorIdAsync(workflow.AlteradoPorId.Value);
            dto.NomeAlterador = alterador?.Nome;
        }

        return dto;
    }
}