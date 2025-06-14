using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using static GestorWorkflow.Core.Services.ServicesMappings;

namespace GestorWorkflow.Core.Services;

public class WorkflowInstanciaService : IWorkflowInstanciaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkflowInstanciaService> _logger;

    public WorkflowInstanciaService(IUnitOfWork unitOfWork, ILogger<WorkflowInstanciaService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WorkflowInstanciaDTO> CriarWorkflowInstanciaAsync(CriarWorkflowInstanciaDTO dto)
    {
        try
        {
            var workflowModelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(dto.WorkflowModeloId);
            if (workflowModelo == null)
                throw new WorkflowModeloNaoEncontradoException(dto.WorkflowModeloId);

            if (!workflowModelo.Ativo)
                throw new WorkflowInstanciaInativaException(dto.WorkflowModeloId);

            var novoId = await GerarProximoIdWorkflowInstanciaAsync();
            var instancia = new WorkflowInstanciaEntity(novoId, dto.WorkflowModeloId, dto.EstadoInicialId, dto.IniciadoPorId);

            var instanciaCriada = await _unitOfWork.WorkflowInstancias.CriarAsync(instancia);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaWorkflowInstanciaDtoAsync(instanciaCriada, _unitOfWork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar workflow instância");
            throw;
        }
    }

    public async Task<WorkflowInstanciaDTO> ObterWorkflowInstanciaPorIdAsync(int id)
    {
        var instancia = await _unitOfWork.WorkflowInstancias.ObterComHistoricoAsync(id);
        if (instancia == null)
            throw new WorkflowInstanciaNaoEncontradaException(id);

        return await MapearParaWorkflowInstanciaDtoAsync(instancia, _unitOfWork);
    }

    public async Task<IEnumerable<WorkflowInstanciaDTO>> ObterTodasWorkflowInstanciasAsync()
    {
        var instancias = await _unitOfWork.WorkflowInstancias.ObterTodosAsync();
        var instanciasDTOs = new List<WorkflowInstanciaDTO>();

        foreach (var instancia in instancias)
            instanciasDTOs.Add(await MapearParaWorkflowInstanciaDtoAsync(instancia, _unitOfWork));

        return instanciasDTOs;
    }

    public async Task<ResultadoOperacaoResponse<RegistoTransicaoDTO>> ExecutarTransicaoAsync(ExecutarTransicaoDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var instancia = await _unitOfWork.WorkflowInstancias.ObterPorIdAsync(dto.WorkflowInstanciaId);
            if (instancia == null)
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Instância de workflow com ID {dto.WorkflowInstanciaId} não encontrada");

            if (!instancia.EstaAtivo())
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Instância de workflow {dto.WorkflowInstanciaId} não está ativa");

            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(dto.TransicaoId);
            if (transicao == null)
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Transição com ID {dto.TransicaoId} não encontrada");

            if (transicao.EstadoOrigemId != instancia.EstadoAtualId)
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Transição não permitida do estado {instancia.EstadoAtualId} para o estado {transicao.EstadoDestinoId}");

            // Validar pré-condições
            if (transicao.PreCondicaoId.HasValue)
            {
                var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(transicao.PreCondicaoId.Value);
                if (preCondicao == null || !preCondicao.Ativo)
                    return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                        $"Pré-condição {transicao.PreCondicaoId} não encontrada ou inativa");

                // TODO: Implementar validação da pré-condição
            }

            // Validar permissões
            if (dto.ExecutadoPorId.HasValue && transicao.PermissoesIds.Any())
            {
                var utilizador = await _unitOfWork.Utilizadores.ObterPorIdAsync(dto.ExecutadoPorId.Value);
                if (utilizador == null || !transicao.PermissoesIds.Any(p => utilizador.TemPermissao(p)))
                    return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                        $"Usuário {dto.ExecutadoPorId} não tem permissão para executar a transição {dto.TransicaoId}");
            }

            // Executar a transição
            instancia.ExecutarTransicao(transicao, dto.ExecutadoPorId);

            // Executar pós-condições
            if (transicao.PosCondicaoId.HasValue)
            {
                var posCondicao = await _unitOfWork.PosCondicoes.ObterPorIdAsync(transicao.PosCondicaoId.Value);
                if (posCondicao == null || !posCondicao.Ativo)
                    return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                        $"Pós-condição {transicao.PosCondicaoId} não encontrada ou inativa");

                // TODO: Implementar execução da pós-condição
            }

            // Verificar se chegou ao estado final
            var estadoDestino = await _unitOfWork.Estados.ObterPorIdAsync(transicao.EstadoDestinoId);
            if (estadoDestino?.EhEstadoFinal() == true)
                instancia.Concluir();

            var instanciaAtualizada = await _unitOfWork.WorkflowInstancias.AtualizarAsync(instancia);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var registro = instanciaAtualizada.HistoricoTransicoes.Last();
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComSucesso(
                await MapearParaRegistoTransicaoDtoAsync(registro, _unitOfWork));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Erro ao executar transição");
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(ex.Message);
        }
    }

    public async Task<WorkflowInstanciaDTO> AtualizarWorkflowInstanciaAsync(int id, AtualizarWorkflowInstanciaDTO dto)
    {
        try
        {
            var instancia = await _unitOfWork.WorkflowInstancias.ObterComHistoricoAsync(id);
            if (instancia == null)
                throw new WorkflowInstanciaNaoEncontradaException(id);

            if (dto.Status.HasValue && dto.Status.Value != instancia.Status)
            {
                await ValidarMudancaStatusAsync(instancia, dto.Status.Value);
                AplicarMudancaStatus(instancia, dto.Status.Value);
            }

            var instanciaAtualizada = await _unitOfWork.WorkflowInstancias.AtualizarAsync(instancia);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaWorkflowInstanciaDtoAsync(instanciaAtualizada, _unitOfWork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar instância de workflow: ID {Id}", id);
            throw;
        }
    }

    private async Task<int> GerarProximoIdWorkflowInstanciaAsync()
    {
        var ultimaInstancia = (await _unitOfWork.WorkflowInstancias.ObterTodosAsync())
            .OrderByDescending(w => w.Id)
            .FirstOrDefault();
        return (ultimaInstancia?.Id ?? 0) + 1;
    }

    private async Task ValidarMudancaStatusAsync(WorkflowInstanciaEntity instancia, StatusWorkflowEntity novoStatus)
    {
        switch (instancia.Status)
        {
            case StatusWorkflowEntity.Ativo:
                if (novoStatus != StatusWorkflowEntity.Suspenso && novoStatus != StatusWorkflowEntity.Cancelado)
                    throw new WorkflowInstanciaInativaException(instancia.Id);
                break;

            case StatusWorkflowEntity.Suspenso:
                if (novoStatus != StatusWorkflowEntity.Ativo && novoStatus != StatusWorkflowEntity.Cancelado)
                    throw new WorkflowInstanciaInativaException(instancia.Id);
                break;

            case StatusWorkflowEntity.Concluido:
            case StatusWorkflowEntity.Cancelado:
                throw new WorkflowInstanciaJaConcluidaException(instancia.Id);
        }

        if (novoStatus == StatusWorkflowEntity.Ativo)
        {
            var modelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(instancia.WorkflowModeloId);
            if (modelo == null || !modelo.Ativo)
                throw new WorkflowModeloInvalidoException(
                    $"O modelo de workflow {instancia.WorkflowModeloId} não está ativo");
        }
    }

    private void AplicarMudancaStatus(WorkflowInstanciaEntity instancia, StatusWorkflowEntity novoStatus)
    {
        switch (novoStatus)
        {
            case StatusWorkflowEntity.Ativo:
                instancia.Reativar();
                break;

            case StatusWorkflowEntity.Suspenso:
                instancia.Suspender();
                break;

            case StatusWorkflowEntity.Cancelado:
                instancia.Cancelar();
                break;

            case StatusWorkflowEntity.Concluido:
                throw new InvalidOperationException(
                    "Para concluir uma instância, use o método ExecutarTransicaoAsync");
        }
    }
} 