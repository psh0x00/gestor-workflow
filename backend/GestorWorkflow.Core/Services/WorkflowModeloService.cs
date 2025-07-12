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

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Cria o modelo primeiro (sem estados)
            var novoId = await GerarProximoIdWorkflowModeloAsync();
            var workflow = new WorkflowModeloEntity(novoId, dto.Nome, null, dto.CriadoPorId);
            if (!string.IsNullOrEmpty(dto.Descricao))
                workflow.AtualizarDescricao(dto.Descricao);
            var workflowCriado = await _unitOfWork.WorkflowModelos.CriarAsync(workflow);
            await _unitOfWork.SaveChangesAsync();

            // 2. Cria os estados, associando ao modelo criado
            // Preenche nomeParaId com nomes normalizados (lowercase, trim)
            var nomeParaId = new Dictionary<string, int>();
            int? estadoInicialId = null;
            var estadosCriados = new List<EstadoEntity>();
            foreach (var estadoDto in dto.Estados)
            {
                var estadoEntity = new EstadoEntity(
                    0,
                    estadoDto.Nome,
                    estadoDto.Tipo,
                    estadoDto.CriadoPorId
                );
                estadoEntity.AtualizarDescricao(estadoDto.Descricao);
                estadoEntity.DefinirCor(estadoDto.CorHexadecimal);
                if (estadoDto.Funcoes != null && estadoDto.Funcoes.Count > 0)
                    estadoEntity.DefinirFuncoes(estadoDto.Funcoes);
                var estadoCriado = await _unitOfWork.CriarEstadoAsync(estadoEntity, workflowCriado.Id);
                await _unitOfWork.SaveChangesAsync();
                nomeParaId[estadoDto.Nome.Trim().ToLowerInvariant()] = estadoCriado.Id;
                estadosCriados.Add(estadoCriado);
                if (estadoDto.IsInicial)
                    estadoInicialId = estadoCriado.Id;
            }
            if (estadoInicialId == null)
                throw new Exception("Nenhum estado inicial definido.");

            // 3. Atualiza o modelo para definir o EstadoInicialId
            workflowCriado = new WorkflowModeloEntity(workflowCriado.Id, workflowCriado.Nome, estadoInicialId, workflowCriado.CriadoPorId);
            if (!string.IsNullOrEmpty(dto.Descricao))
                workflowCriado.AtualizarDescricao(dto.Descricao);
            foreach (var estado in estadosCriados)
                workflowCriado.AdicionarEstado(estado);
            await _unitOfWork.WorkflowModelos.AtualizarAsync(workflowCriado);
            await _unitOfWork.SaveChangesAsync();

            // 4. Cria as transições
            if (dto.Transicoes != null)
            {
                foreach (var transicaoDto in dto.Transicoes)
                {
                    int origemId = 0, destinoId = 0;
                    if (transicaoDto.EstadoOrigemId.HasValue && transicaoDto.EstadoOrigemId > 0)
                        origemId = transicaoDto.EstadoOrigemId.Value;
                    if (transicaoDto.EstadoDestinoId > 0)
                        destinoId = transicaoDto.EstadoDestinoId;
                    // Se vierem nomes, converte para IDs
                    string? nomeOrigem = transicaoDto.NomeEstadoOrigem;
                    string? nomeDestino = transicaoDto.NomeEstadoDestino;
                    if (string.IsNullOrEmpty(nomeOrigem))
                    {
                        var prop = transicaoDto.GetType().GetProperty("origem", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (prop != null)
                            nomeOrigem = prop.GetValue(transicaoDto)?.ToString();
                    }
                    if (string.IsNullOrEmpty(nomeDestino))
                    {
                        var prop = transicaoDto.GetType().GetProperty("destino", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (prop != null)
                            nomeDestino = prop.GetValue(transicaoDto)?.ToString();
                    }
                    if (origemId == 0 && !string.IsNullOrEmpty(nomeOrigem))
                    {
                        var key = nomeOrigem.Trim().ToLowerInvariant();
                        if (nomeParaId.ContainsKey(key))
                            origemId = nomeParaId[key];
                    }
                    if (destinoId == 0 && !string.IsNullOrEmpty(nomeDestino))
                    {
                        var key = nomeDestino.Trim().ToLowerInvariant();
                        if (nomeParaId.ContainsKey(key))
                            destinoId = nomeParaId[key];
                    }

                    // Log para debug
                    _logger.LogDebug($"nomeParaId: {string.Join(", ", nomeParaId.Select(kv => $"{kv.Key}={kv.Value}"))}");
                    _logger.LogDebug($"Transição recebida: Origem='{transicaoDto.NomeEstadoOrigem}', Destino='{transicaoDto.NomeEstadoDestino}', origemId={origemId}, destinoId={destinoId}");

                    var transicaoEntity = new TransicaoEntity(0, destinoId, origemId);
                    if (!string.IsNullOrEmpty(transicaoDto.Nome))
                        transicaoEntity.DefinirNome(transicaoDto.Nome);
                    if (!string.IsNullOrEmpty(transicaoDto.Descricao))
                        transicaoEntity.DefinirDescricao(transicaoDto.Descricao);
                    if (transicaoDto.PreCondicaoId.HasValue)
                        transicaoEntity.DefinirPreCondicao(transicaoDto.PreCondicaoId);
                    if (transicaoDto.PosCondicaoId.HasValue)
                        transicaoEntity.DefinirPosCondicao(transicaoDto.PosCondicaoId);
                    if (transicaoDto.PermissoesIds != null)
                    {
                        foreach (var pid in transicaoDto.PermissoesIds)
                            transicaoEntity.AdicionarPermissao(pid);
                    }
                    // Garante que o WorkflowModeloId está setado
                    transicaoEntity.WorkflowModeloId = workflowCriado.Id;
                    if (origemId == 0 || destinoId == 0)
                        throw new Exception($"Transição inválida: origemId={origemId}, destinoId={destinoId}. Verifique se os nomes dos estados estão corretos e se todos os estados foram criados. Origem: '{transicaoDto.NomeEstadoOrigem}', Destino: '{transicaoDto.NomeEstadoDestino}'");
                    await _unitOfWork.Transicoes.CriarAsync(transicaoEntity);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitTransactionAsync();
            _logger.LogInformation("Workflow modelo criado com sucesso: ID {Id}", workflowCriado.Id);
            return await MapearParaWorkflowModeloDtoAsync(workflowCriado);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Erro ao criar workflow modelo");
            throw;
        }
    }

    public async Task<WorkflowModeloDTO> ObterWorkflowModeloPorIdAsync(int id)
    {
        var workflow = await _unitOfWork.WorkflowModelos.ObterComDetalhesAsync(id);
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
        if (workflow.EstadoInicialId.HasValue)
        {
            var estadoInicial = await _unitOfWork.Estados.ObterPorIdAsync(workflow.EstadoInicialId.Value);
            dto.NomeEstadoInicial = estadoInicial?.Nome;
        }
        else
        {
            dto.NomeEstadoInicial = null;
        }

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