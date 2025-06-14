using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestorWorkflow.Core.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(IUnitOfWork unitOfWork, ILogger<WorkflowService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Estados

    public async Task<EstadoDTO> CriarEstadoAsync(CriarEstadoDTO dto)
    {
        try
        {
            _logger.LogInformation("Criando novo estado: {Nome}", dto.Nome);

            // Validar se já existe um estado com o mesmo nome
            if (await _unitOfWork.Estados.ExisteNomeAsync(dto.Nome))
                throw new EstadoNomeJaExisteException(dto.Nome);

            // Gerar ID único (ou usar auto-increment da base de dados)
            var novoId = await GerarProximoIdEstadoAsync();

            var estado = new EstadoEntity(novoId, dto.Nome, dto.Tipo, dto.CriadoPorId);

            if (!string.IsNullOrEmpty(dto.Descricao))
                estado.AtualizarDescricao(dto.Descricao);

            if (!string.IsNullOrEmpty(dto.CorHexadecimal))
                estado.DefinirCor(dto.CorHexadecimal);

            var estadoCriado = await _unitOfWork.Estados.CriarAsync(estado);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Estado criado com sucesso: ID {Id}", estadoCriado.Id);

            return MapearParaEstadoDto(estadoCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar estado: {Nome}", dto.Nome);
            throw;
        }
    }

    public async Task<EstadoDTO> ObterEstadoPorIdAsync(int id)
    {
        var estado = await _unitOfWork.Estados.ObterPorIdAsync(id);
        if (estado == null) throw new EstadoNaoEncontradoException(id);

        return MapearParaEstadoDto(estado);
    }

    public async Task<IEnumerable<EstadoDTO>> ObterTodosEstadosAsync()
    {
        var estados = await _unitOfWork.Estados.ObterTodosAsync();
        return estados.Select(MapearParaEstadoDto);
    }

    public async Task<EstadoDTO> AtualizarEstadoAsync(int id, AtualizarEstadoDTO dto)
    {
        try
        {
            var estado = await _unitOfWork.Estados.ObterPorIdAsync(id);
            if (estado == null) throw new KeyNotFoundException($"Estado com ID {id} não encontrado");

            if (!string.IsNullOrEmpty(dto.Descricao))
                estado.AtualizarDescricao(dto.Descricao);

            if (!string.IsNullOrEmpty(dto.CorHexadecimal))
                estado.DefinirCor(dto.CorHexadecimal);

            if (dto.Ativo.HasValue)
            {
                if (dto.Ativo.Value)
                    estado.Ativar();
                else
                    estado.Desativar();
            }

            var estadoAtualizado = await _unitOfWork.Estados.AtualizarAsync(estado);
            await _unitOfWork.SaveChangesAsync();

            return MapearParaEstadoDto(estadoAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estado: ID {Id}", id);
            throw;
        }
    }

    public async Task RemoverEstadoAsync(int id)
    {
        try
        {
            if (!await _unitOfWork.Estados.ExisteAsync(id))
                throw new EstadoNaoEncontradoException(id);

            // Verificar se o estado está sendo usado em alguma transição
            var transicoesOrigem = await _unitOfWork.Transicoes.ObterPorEstadoOrigemAsync(id);
            var transicoesDestino = await _unitOfWork.Transicoes.ObterPorEstadoDestinoAsync(id);

            if (transicoesOrigem.Any() || transicoesDestino.Any())
                throw new EstadoEmUsoException(id);

            await _unitOfWork.Estados.RemoverAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Estado removido com sucesso: ID {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover estado: ID {Id}", id);
            throw;
        }
    }

    private async Task<string?> ObterNomeEstadoAsync(int? estadoId)
    {
        if (!estadoId.HasValue) return null;

        var estado = await _unitOfWork.Estados.ObterPorIdAsync(estadoId.Value);
        return estado?.Nome;
    }

    #endregion

    #region Transições

    public async Task<TransicaoDTO> CriarTransicaoAsync(CriarTransicaoDTO dto)
    {
        try
        {
            _logger.LogInformation("Criando nova transição");

            // Validar se os estados existem
            if (dto.EstadoOrigemId.HasValue && !await _unitOfWork.Estados.ExisteAsync(dto.EstadoOrigemId.Value))
                throw new EstadoNaoEncontradoException(dto.EstadoOrigemId.Value);

            if (!await _unitOfWork.Estados.ExisteAsync(dto.EstadoDestinoId))
                throw new InvalidOperationException($"Estado de destino com ID {dto.EstadoDestinoId} não encontrado");

            // Validar pré e pós-condições se especificadas
            if (dto.PreCondicaoId.HasValue && !await _unitOfWork.PreCondicoes.ExisteAsync(dto.PreCondicaoId.Value))
                throw new CondicaoNaoEncontradaException(dto.PreCondicaoId.Value, "Pré-condição");

            if (dto.PosCondicaoId.HasValue && !await _unitOfWork.PosCondicoes.ExisteAsync(dto.PosCondicaoId.Value))
                throw new CondicaoNaoEncontradaException(dto.PosCondicaoId.Value, "Pós-condição");

            var novoId = await GerarProximoIdTransicaoAsync();
            var transicao = new TransicaoEntity(novoId, dto.EstadoDestinoId, dto.EstadoOrigemId);

            if (!string.IsNullOrEmpty(dto.Nome))
                transicao.DefinirNome(dto.Nome);

            if (!string.IsNullOrEmpty(dto.Descricao))
                transicao.DefinirDescricao(dto.Descricao);

            if (dto.PreCondicaoId.HasValue)
                transicao.DefinirPreCondicao(dto.PreCondicaoId.Value);

            if (dto.PosCondicaoId.HasValue)
                transicao.DefinirPosCondicao(dto.PosCondicaoId.Value);

            // Adicionar permissões
            foreach (var permissaoId in dto.PermissoesIds)
                if (await _unitOfWork.Permissoes.ExisteAsync(permissaoId))
                    transicao.AdicionarPermissao(permissaoId);

            var transicaoCriada = await _unitOfWork.Transicoes.CriarAsync(transicao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaTransicaoDtoAsync(transicaoCriada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar transição");
            throw;
        }
    }

    public async Task<TransicaoDTO> ObterTransicaoPorIdAsync(int id)
    {
        var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(id);
        if (transicao == null) throw new TransicaoNaoEncontradaException(id);

        return await MapearParaTransicaoDtoAsync(transicao);
    }

    public async Task<IEnumerable<TransicaoDTO>> ObterTodasTransicoesAsync()
    {
        var transicoes = await _unitOfWork.Transicoes.ObterTodosAsync();
        var transicoesDTOs = new List<TransicaoDTO>();

        foreach (var transicao in transicoes) transicoesDTOs.Add(await MapearParaTransicaoDtoAsync(transicao));

        return transicoesDTOs;
    }

    public async Task<TransicaoDTO> AtualizarTransicaoAsync(int id, AtualizarTransicaoDTO dto)
    {
        try
        {
            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(id);
            if (transicao == null) throw new KeyNotFoundException($"Transição com ID {id} não encontrada");

            if (!string.IsNullOrEmpty(dto.Nome))
                transicao.DefinirNome(dto.Nome);

            if (!string.IsNullOrEmpty(dto.Descricao))
                transicao.DefinirDescricao(dto.Descricao);

            if (dto.PreCondicaoId.HasValue)
                transicao.DefinirPreCondicao(dto.PreCondicaoId.Value);

            if (dto.PosCondicaoId.HasValue)
                transicao.DefinirPosCondicao(dto.PosCondicaoId.Value);

            // Atualizar permissões se especificadas
            if (dto.PermissoesIds != null)
            {
                // Remover todas as permissões existentes
                var permissoesAtuais = transicao.PermissoesIds.ToList();
                foreach (var permissaoId in permissoesAtuais) transicao.RemoverPermissao(permissaoId);

                // Adicionar as novas permissões
                foreach (var permissaoId in dto.PermissoesIds)
                    if (await _unitOfWork.Permissoes.ExisteAsync(permissaoId))
                        transicao.AdicionarPermissao(permissaoId);
            }

            var transicaoAtualizada = await _unitOfWork.Transicoes.AtualizarAsync(transicao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaTransicaoDtoAsync(transicaoAtualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar transição: ID {Id}", id);
            throw;
        }
    }

    public async Task RemoverTransicaoAsync(int id)
    {
        try
        {
            if (!await _unitOfWork.Transicoes.ExisteAsync(id))
                throw new TransicaoNaoEncontradaException(id);

            await _unitOfWork.Transicoes.RemoverAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Transição removida com sucesso: ID {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover transição: ID {Id}", id);
            throw;
        }
    }

    #endregion

    #region Permissões

    public async Task<PermissaoDTO> CriarPermissaoAsync(CriarPermissaoDTO dto)
    {
        try
        {
            if (await _unitOfWork.Permissoes.ExisteNomeAsync(dto.Nome))
                throw new PermissaoJaExisteException(dto.Nome, dto.TransicaoId);

            if (!await _unitOfWork.Transicoes.ExisteAsync(dto.TransicaoId))
                throw new TransicaoNaoEncontradaException(dto.TransicaoId);

            var novoId = await GerarProximoIdPermissaoAsync();
            var permissao = new PermissaoEntity(novoId, dto.Nome, dto.TransicaoId);

            if (!string.IsNullOrEmpty(dto.Descricao))
                permissao.AtualizarDescricao(dto.Descricao);

            var permissaoCriada = await _unitOfWork.Permissoes.CriarAsync(permissao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaPermissaoDtoAsync(permissaoCriada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar permissão: {Nome}", dto.Nome);
            throw;
        }
    }

    public async Task<PermissaoDTO> ObterPermissaoPorIdAsync(int id)
    {
        var permissao = await _unitOfWork.Permissoes.ObterPorIdAsync(id);
        if (permissao == null) throw new PermissaoNaoEncontradaException(id);

        return await MapearParaPermissaoDtoAsync(permissao);
    }

    public async Task<IEnumerable<PermissaoDTO>> ObterTodasPermissoesAsync()
    {
        var permissoes = await _unitOfWork.Permissoes.ObterTodosAsync();
        var permissoesDTOs = new List<PermissaoDTO>();

        foreach (var permissao in permissoes) permissoesDTOs.Add(await MapearParaPermissaoDtoAsync(permissao));

        return permissoesDTOs;
    }

    public async Task<PermissaoDTO> AtualizarPermissaoAsync(int id, AtualizarPermissaoDTO dto)
    {
        try
        {
            var permissao = await _unitOfWork.Permissoes.ObterPorIdAsync(id);
            if (permissao == null) throw new KeyNotFoundException($"Permissão com ID {id} não encontrada");

            if (!string.IsNullOrEmpty(dto.Descricao))
                permissao.AtualizarDescricao(dto.Descricao);

            var permissaoAtualizada = await _unitOfWork.Permissoes.AtualizarAsync(permissao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaPermissaoDtoAsync(permissaoAtualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar permissão: ID {Id}", id);
            throw;
        }
    }

    public async Task RemoverPermissaoAsync(int id)
    {
        try
        {
            if (!await _unitOfWork.Permissoes.ExisteAsync(id))
                throw new PermissaoNaoEncontradaException(id);

            await _unitOfWork.Permissoes.RemoverAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Permissão removida com sucesso: ID {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover permissão: ID {Id}", id);
            throw;
        }
    }

    #endregion

    #region Workflow Modelos

    public async Task<WorkflowModeloDTO> CriarWorkflowModeloAsync(CriarWorkflowModeloDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            if (await _unitOfWork.WorkflowModelos.ExisteNomeAsync(dto.Nome))
                throw new WorkflowModeloNomeJaExisteException(dto.Nome);

            var novoId = await GerarProximoIdWorkflowModeloAsync();
            var workflowModelo = new WorkflowModeloEntity(novoId, dto.Nome, dto.EstadoInicialId, dto.CriadoPorId);

            if (!string.IsNullOrEmpty(dto.Descricao))
                workflowModelo.AtualizarDescricao(dto.Descricao);

            // Criar estados associados
            foreach (var estadoDto in dto.Estados)
            {
                var estadoId = await GerarProximoIdEstadoAsync();
                var estado = new EstadoEntity(estadoId, estadoDto.Nome, estadoDto.Tipo, estadoDto.CriadoPorId);

                if (!string.IsNullOrEmpty(estadoDto.Descricao))
                    estado.AtualizarDescricao(estadoDto.Descricao);

                if (!string.IsNullOrEmpty(estadoDto.CorHexadecimal))
                    estado.DefinirCor(estadoDto.CorHexadecimal);

                workflowModelo.AdicionarEstado(estado);
                await _unitOfWork.Estados.CriarAsync(estado);
            }

            // Criar transições associadas
            foreach (var transicaoDto in dto.Transicoes)
            {
                var transicaoId = await GerarProximoIdTransicaoAsync();
                var transicao = new TransicaoEntity(transicaoId, transicaoDto.EstadoDestinoId, transicaoDto.EstadoOrigemId);

                if (!string.IsNullOrEmpty(transicaoDto.Nome))
                    transicao.DefinirNome(transicaoDto.Nome);

                if (!string.IsNullOrEmpty(transicaoDto.Descricao))
                    transicao.DefinirDescricao(transicaoDto.Descricao);

                if (transicaoDto.PreCondicaoId.HasValue)
                    transicao.DefinirPreCondicao(transicaoDto.PreCondicaoId.Value);

                if (transicaoDto.PosCondicaoId.HasValue)
                    transicao.DefinirPosCondicao(transicaoDto.PosCondicaoId.Value);

                foreach (var permissaoId in transicaoDto.PermissoesIds) transicao.AdicionarPermissao(permissaoId);

                workflowModelo.AdicionarTransicao(transicao);
                await _unitOfWork.Transicoes.CriarAsync(transicao);
            }

            // Validar o workflow antes de salvar
            if (!workflowModelo.ValidarWorkflow())
                throw new WorkflowModeloInvalidoException("Workflow modelo não passou na validação");

            var workflowCriado = await _unitOfWork.WorkflowModelos.CriarAsync(workflowModelo);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await MapearParaWorkflowModeloDtoAsync(workflowCriado);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Erro ao criar workflow modelo: {Nome}", dto.Nome);
            throw;
        }
    }

    public async Task<WorkflowModeloDTO> ObterWorkflowModeloPorIdAsync(int id)
    {
        var workflowModelo = await _unitOfWork.WorkflowModelos.ObterComDetalhesAsync(id);
        if (workflowModelo == null) throw new WorkflowModeloNaoEncontradoException(id);

        return await MapearParaWorkflowModeloDtoAsync(workflowModelo);
    }

    public async Task<IEnumerable<WorkflowModeloDTO>> ObterTodosWorkflowModelosAsync()
    {
        var workflowModelos = await _unitOfWork.WorkflowModelos.ObterTodosAsync();
        var workflowModelosDTOs = new List<WorkflowModeloDTO>();

        foreach (var workflow in workflowModelos)
            workflowModelosDTOs.Add(await MapearParaWorkflowModeloDtoAsync(workflow));

        return workflowModelosDTOs;
    }

    public async Task<WorkflowModeloDTO> AtualizarWorkflowModeloAsync(int id, AtualizarWorkflowModeloDTO dto)
    {
        try
        {
            var workflowModelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(id);
            if (workflowModelo == null) throw new KeyNotFoundException($"Workflow modelo com ID {id} não encontrado");

            if (!string.IsNullOrEmpty(dto.Descricao))
                workflowModelo.AtualizarDescricao(dto.Descricao);

            if (!string.IsNullOrEmpty(dto.Versao) && dto.AlteradoPorId.HasValue)
                workflowModelo.AtualizarVersao(dto.Versao, dto.AlteradoPorId.Value);

            if (dto.Ativo.HasValue)
            {
                if (dto.Ativo.Value)
                    workflowModelo.Ativar();
                else
                    workflowModelo.Desativar();
            }

            var workflowAtualizado = await _unitOfWork.WorkflowModelos.AtualizarAsync(workflowModelo);
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
            if (instancias.Any(i => i.EstaAtivo()))
                throw new WorkflowModeloComInstanciasAtivasException(id);

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

    #endregion

    #region Workflow Instâncias

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

            return await MapearParaWorkflowInstanciaDtoAsync(instanciaCriada);
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
        if (instancia == null) throw new WorkflowInstanciaNaoEncontradaException(id);

        return await MapearParaWorkflowInstanciaDtoAsync(instancia);
    }

    public async Task<IEnumerable<WorkflowInstanciaDTO>> ObterTodasWorkflowInstanciasAsync()
    {
        var instancias = await _unitOfWork.WorkflowInstancias.ObterTodosAsync();
        var instanciasDTOs = new List<WorkflowInstanciaDTO>();

        foreach (var instancia in instancias) instanciasDTOs.Add(await MapearParaWorkflowInstanciaDtoAsync(instancia));

        return instanciasDTOs;
    }

public async Task<ResultadoOperacaoResponse<RegistoTransicaoDTO>> ExecutarTransicaoAsync(ExecutarTransicaoDTO dto)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();

        // Validar instância de workflow
        var instancia = await _unitOfWork.WorkflowInstancias.ObterPorIdAsync(dto.WorkflowInstanciaId);
        if (instancia == null)
        {
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                $"Instância de workflow com ID {dto.WorkflowInstanciaId} não encontrada");
        }

        if (!instancia.EstaAtivo())
        {
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                $"Instância de workflow {dto.WorkflowInstanciaId} não está ativa");
        }

        // Validar transição
        var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(dto.TransicaoId);
        if (transicao == null)
        {
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                $"Transição com ID {dto.TransicaoId} não encontrada");
        }

        // Verificar se a transição é válida para o estado atual
        if (transicao.EstadoOrigemId != instancia.EstadoAtualId)
        {
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                $"Transição não permitida do estado {instancia.EstadoAtualId} para o estado {transicao.EstadoDestinoId}");
        }

        // Validar pré-condições
        if (transicao.PreCondicaoId.HasValue)
        {
            var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(transicao.PreCondicaoId.Value);
            if (preCondicao == null)
            {
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Pré-condição com ID {transicao.PreCondicaoId.Value} não encontrada");
            }

            if (!preCondicao.Ativo)
            {
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Pré-condição {preCondicao.Nome} está inativa");
            }

            // Aqui você pode adicionar lógica para executar a validação SQL da pré-condição
            // if (!await ValidarPreCondicaoSql(preCondicao.CondicaoSql))
            // {
            //     return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
            //         $"Pré-condição {preCondicao.Nome} não foi atendida");
            // }
        }

        // Validar permissões
        if (dto.ExecutadoPorId.HasValue && transicao.PermissoesIds.Any())
        {
            var utilizador = await _unitOfWork.Utilizadores.ObterPorIdAsync(dto.ExecutadoPorId.Value);
            if (utilizador == null)
            {
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Utilizador com ID {dto.ExecutadoPorId.Value} não encontrado");
            }

            var temPermissao = transicao.PermissoesIds.Any(p => utilizador.TemPermissao(p));
            if (!temPermissao)
            {
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Utilizador {utilizador.Nome} não tem permissão para executar esta transição");
            }
        }

        // Executar a transição
        instancia.ExecutarTransicao(transicao, dto.ExecutadoPorId);

        // Executar pós-condições
        if (transicao.PosCondicaoId.HasValue)
        {
            var posCondicao = await _unitOfWork.PosCondicoes.ObterPorIdAsync(transicao.PosCondicaoId.Value);
            if (posCondicao == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Pós-condição com ID {transicao.PosCondicaoId.Value} não encontrada");
            }

            if (!posCondicao.Ativo)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                    $"Pós-condição {posCondicao.Nome} está inativa");
            }

            // Aqui você pode adicionar lógica para executar a ação SQL da pós-condição
            // if (!await ExecutarPosCondicaoSql(posCondicao.AcaoSql))
            // {
            //     await _unitOfWork.RollbackTransactionAsync();
            //     return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
            //         $"Falha ao executar pós-condição {posCondicao.Nome}");
            // }
        }

        // Verificar se chegou a um estado final
        var estadoDestino = await _unitOfWork.Estados.ObterPorIdAsync(transicao.EstadoDestinoId);
        if (estadoDestino != null && estadoDestino.EhEstadoFinal())
        {
            instancia.Concluir();
        }

        // Salvar mudanças
        await _unitOfWork.WorkflowInstancias.AtualizarAsync(instancia);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Criar o DTO de retorno com os dados da transição executada
        var ultimaTransicao = instancia.HistoricoTransicoes.LastOrDefault();
        if (ultimaTransicao == null)
        {
            return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
                "Erro interno: Não foi possível obter o registro da transição executada");
        }

        var registoTransicaoDTO = new RegistoTransicaoDTO
        {
            TransicaoId = ultimaTransicao.TransicaoId,
            EstadoOrigemId = ultimaTransicao.EstadoOrigemId,
            EstadoDestinoId = ultimaTransicao.EstadoDestinoId,
            DataExecucao = ultimaTransicao.DataExecucao,
            ExecutadoPorId = ultimaTransicao.ExecutadoPorId,
            Sucesso = ultimaTransicao.Sucesso,
            MensagemErro = ultimaTransicao.MensagemErro,
            NomeTransicao = transicao.Nome,
            NomeEstadoOrigem = await ObterNomeEstadoAsync(ultimaTransicao.EstadoOrigemId),
            NomeEstadoDestino = await ObterNomeEstadoAsync(ultimaTransicao.EstadoDestinoId),
            NomeExecutor = dto.ExecutadoPorId.HasValue ? await ObterNomeUtilizadorAsync(dto.ExecutadoPorId.Value) : null
        };

        return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComSucesso(
            registoTransicaoDTO,
            $"Transição executada com sucesso. Estado alterado para {estadoDestino?.Nome ?? "Desconhecido"}");
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackTransactionAsync();
        _logger.LogError(ex, "Erro ao executar transição: TransicaoId {TransicaoId}, InstanciaId {InstanciaId}",
            dto.TransicaoId, dto.WorkflowInstanciaId);

        return ResultadoOperacaoResponse<RegistoTransicaoDTO>.ComErro(
            $"Erro interno do sistema ao executar transição: {ex.Message}");
    }
}

    public async Task<WorkflowInstanciaDTO> AtualizarWorkflowInstanciaAsync(int id, AtualizarWorkflowInstanciaDTO dto)
    {
        try
        {
            // 1. Obter a instância existente
            var instancia = await _unitOfWork.WorkflowInstancias.ObterComHistoricoAsync(id);
            if (instancia == null) throw new WorkflowInstanciaNaoEncontradaException(id);

            // 2. Validar e aplicar mudanças de status
            if (dto.Status.HasValue && dto.Status.Value != instancia.Status)
                switch (dto.Status.Value)
                {
                    case StatusWorkflowEntity.Ativo:
                        if (instancia.EstaSuspenso())
                            instancia.Reativar();
                        else
                            throw new InvalidOperationException("Só é possível reativar workflows suspensos");
                        break;

                    case StatusWorkflowEntity.Suspenso:
                        if (instancia.EstaAtivo())
                            instancia.Suspender();
                        else
                            throw new InvalidOperationException("Só é possível suspender workflows ativos");
                        break;

                    case StatusWorkflowEntity.Cancelado:
                        if (instancia.EstaAtivo() || instancia.EstaSuspenso())
                            instancia.Cancelar();
                        else
                            throw new InvalidOperationException("Só é possível cancelar workflows ativos ou suspensos");
                        break;

                    case StatusWorkflowEntity.Concluido:
                        throw new InvalidOperationException(
                            "Não é possível marcar manualmente um workflow como concluído");

                    default:
                        throw new ArgumentOutOfRangeException(nameof(dto.Status), "Status de workflow inválido");
                }

            // 3. Salvar as alterações
            var instanciaAtualizada = await _unitOfWork.WorkflowInstancias.AtualizarAsync(instancia);
            await _unitOfWork.SaveChangesAsync();

            // 4. Converter para DTO e retornar
            return await MapearParaWorkflowInstanciaDtoAsync(instanciaAtualizada);
        }
        catch (WorkflowInstanciaNaoEncontradaException)
        {
            throw; // Re-lançar exceções específicas
        }
        catch (InvalidOperationException ex)
        {
            throw new WorkflowInstanciaInativaException(id); // Ou outra exceção mais específica
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar instância de workflow: ID {Id}", id);
            throw new WorkflowInstanciaInativaException(id);
        }
    }


    public async Task<bool> ValidarTransicaoAsync(int workflowInstanciaId, int transicaoId, int? utilizadorId = null)
    {
        try
        {
            var instancia = await _unitOfWork.WorkflowInstancias.ObterPorIdAsync(workflowInstanciaId);
            if (instancia == null || !instancia.EstaAtivo()) return false;

            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(transicaoId);
            if (transicao == null || transicao.EstadoOrigemId != instancia.EstadoAtualId) return false;

            // Validar pré-condições
            if (transicao.PreCondicaoId.HasValue)
            {
                var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(transicao.PreCondicaoId.Value);
                if (preCondicao == null || !preCondicao.Ativo) return false;
            }

            // Validar permissões se houver usuário
            if (utilizadorId.HasValue && transicao.PermissoesIds.Any())
            {
                var utilizador = await _unitOfWork.Utilizadores.ObterPorIdAsync(utilizadorId.Value);
                if (utilizador == null || !transicao.PermissoesIds.Any(p => utilizador.TemPermissao(p))) return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar transição");
            return false;
        }
    }

    public async Task<IEnumerable<TransicaoDTO>> ObterTransicoesPossiveisAsync(int workflowInstanciaId)
    {
        try
        {
            var instancia = await _unitOfWork.WorkflowInstancias.ObterPorIdAsync(workflowInstanciaId);
            if (instancia == null)
                throw new KeyNotFoundException($"Workflow instância com ID {workflowInstanciaId} não encontrada");

            var workflowModelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(instancia.WorkflowModeloId);
            if (workflowModelo == null)
                throw new InvalidOperationException(
                    $"Workflow modelo não encontrado para a instância {workflowInstanciaId}");

            var transicoes = workflowModelo.ObterTransicoesPossiveis(instancia.EstadoAtualId ?? 0);
            var transicoesDTOs = new List<TransicaoDTO>();

            foreach (var transicao in transicoes) transicoesDTOs.Add(await MapearParaTransicaoDtoAsync(transicao));

            return transicoesDTOs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter transições possíveis para a instância {InstanciaId}",
                workflowInstanciaId);
            throw;
        }
    }

    public async Task<bool> UtilizadorTemPermissaoAsync(int utilizadorId, int permissaoId)
    {
        try
        {
            var utilizador = await _unitOfWork.Utilizadores.ObterPorIdAsync(utilizadorId);
            if (utilizador == null) return false;

            return utilizador.TemPermissao(permissaoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar permissão do usuário");
            return false;
        }
    }

    #endregion

    #region Métodos Auxiliares

    private async Task<int> GerarProximoIdEstadoAsync()
    {
        var ultimoEstado = (await _unitOfWork.Estados.ObterTodosAsync()).OrderByDescending(e => e.Id).FirstOrDefault();
        return (ultimoEstado?.Id ?? 0) + 1;
    }

    private async Task<int> GerarProximoIdTransicaoAsync()
    {
        var ultimaTransicao = (await _unitOfWork.Transicoes.ObterTodosAsync()).OrderByDescending(t => t.Id)
            .FirstOrDefault();
        return (ultimaTransicao?.Id ?? 0) + 1;
    }

    private async Task<int> GerarProximoIdPermissaoAsync()
    {
        var ultimaPermissao = (await _unitOfWork.Permissoes.ObterTodosAsync()).OrderByDescending(p => p.Id)
            .FirstOrDefault();
        return (ultimaPermissao?.Id ?? 0) + 1;
    }

    private async Task<int> GerarProximoIdWorkflowModeloAsync()
    {
        var ultimoModelo = (await _unitOfWork.WorkflowModelos.ObterTodosAsync()).OrderByDescending(w => w.Id)
            .FirstOrDefault();
        return (ultimoModelo?.Id ?? 0) + 1;
    }

    private async Task<int> GerarProximoIdWorkflowInstanciaAsync()
    {
        var ultimaInstancia = (await _unitOfWork.WorkflowInstancias.ObterTodosAsync()).OrderByDescending(w => w.Id)
            .FirstOrDefault();
        return (ultimaInstancia?.Id ?? 0) + 1;
    }

    private EstadoDTO MapearParaEstadoDto(EstadoEntity estadoEntity)
    {
        return new EstadoDTO
        {
            Id = estadoEntity.Id,
            Nome = estadoEntity.Nome,
            Descricao = estadoEntity.Descricao,
            Tipo = estadoEntity.Tipo,
            CorHexadecimal = estadoEntity.CorHexadecimal,
            Ativo = estadoEntity.Ativo,
            DataCriacao = estadoEntity.DataCriacao,
            CriadoPorId = estadoEntity.CriadoPorId
        };
    }

    private async Task<TransicaoDTO> MapearParaTransicaoDtoAsync(TransicaoEntity transicaoEntity)
    {
        var dto = new TransicaoDTO
        {
            Id = transicaoEntity.Id,
            Nome = transicaoEntity.Nome,
            Descricao = transicaoEntity.Descricao,
            EstadoOrigemId = transicaoEntity.EstadoOrigemId,
            EstadoDestinoId = transicaoEntity.EstadoDestinoId,
            PreCondicaoId = transicaoEntity.PreCondicaoId,
            PosCondicaoId = transicaoEntity.PosCondicaoId,
            PermissoesIds = transicaoEntity.PermissoesIds.ToList()
        };

        // Adicionar informações de navegação se disponíveis
        if (transicaoEntity.EstadoOrigemId.HasValue)
        {
            var estadoOrigem = await _unitOfWork.Estados.ObterPorIdAsync(transicaoEntity.EstadoOrigemId.Value);
            dto.NomeEstadoOrigem = estadoOrigem?.Nome;
        }

        var estadoDestino = await _unitOfWork.Estados.ObterPorIdAsync(transicaoEntity.EstadoDestinoId);
        dto.NomeEstadoDestino = estadoDestino?.Nome;

        if (transicaoEntity.PreCondicaoId.HasValue)
        {
            var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(transicaoEntity.PreCondicaoId.Value);
            dto.NomePreCondicao = preCondicao?.Nome;
        }

        if (transicaoEntity.PosCondicaoId.HasValue)
        {
            var posCondicao = await _unitOfWork.PosCondicoes.ObterPorIdAsync(transicaoEntity.PosCondicaoId.Value);
            dto.NomePosCondicao = posCondicao?.Nome;
        }

        return dto;
    }

    private async Task<PermissaoDTO> MapearParaPermissaoDtoAsync(PermissaoEntity permissaoEntity)
    {
        var dto = new PermissaoDTO
        {
            Id = permissaoEntity.Id,
            Nome = permissaoEntity.Nome,
            Descricao = permissaoEntity.Descricao,
            TransicaoId = permissaoEntity.TransicaoId
        };

        var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(permissaoEntity.TransicaoId);
        if (transicao != null) dto.NomeTransicao = transicao.Nome;

        return dto;
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
        foreach (var transicao in workflow.Transicoes) dto.Transicoes.Add(await MapearParaTransicaoDtoAsync(transicao));

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

    private async Task<WorkflowInstanciaDTO> MapearParaWorkflowInstanciaDtoAsync(WorkflowInstanciaEntity instanciaEntity)
    {
        var dto = new WorkflowInstanciaDTO
        {
            Id = instanciaEntity.Id,
            WorkflowModeloId = instanciaEntity.WorkflowModeloId,
            Status = instanciaEntity.Status,
            EstadoAtualId = instanciaEntity.EstadoAtualId,
            DataInicio = instanciaEntity.DataInicio,
            DataFim = instanciaEntity.DataFim,
            IniciadoPorId = instanciaEntity.IniciadoPorId
        };

        // Mapear histórico de transições
        foreach (var registro in instanciaEntity.HistoricoTransicoes)
        {
            var registroDto = new RegistoTransicaoDTO
            {
                TransicaoId = registro.TransicaoId,
                EstadoOrigemId = registro.EstadoOrigemId,
                EstadoDestinoId = registro.EstadoDestinoId,
                DataExecucao = registro.DataExecucao,
                ExecutadoPorId = registro.ExecutadoPorId,
                Sucesso = registro.Sucesso,
                MensagemErro = registro.MensagemErro
            };

            // Adicionar informações adicionais
            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(registro.TransicaoId);
            registroDto.NomeTransicao = transicao?.Nome;

            if (registro.EstadoOrigemId.HasValue)
            {
                var estadoOrigem = await _unitOfWork.Estados.ObterPorIdAsync(registro.EstadoOrigemId.Value);
                registroDto.NomeEstadoOrigem = estadoOrigem?.Nome;
            }

            var estadoDestino = await _unitOfWork.Estados.ObterPorIdAsync(registro.EstadoDestinoId);
            registroDto.NomeEstadoDestino = estadoDestino?.Nome;

            if (registro.ExecutadoPorId.HasValue)
            {
                var executor = await _unitOfWork.Utilizadores.ObterPorIdAsync(registro.ExecutadoPorId.Value);
                registroDto.NomeExecutor = executor?.Nome;
            }

            dto.HistoricoTransicoes.Add(registroDto);
        }

        // Adicionar informações adicionais
        var workflowModelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(instanciaEntity.WorkflowModeloId);
        dto.NomeWorkflowModelo = workflowModelo?.Nome;

        if (instanciaEntity.EstadoAtualId.HasValue)
        {
            var estadoAtual = await _unitOfWork.Estados.ObterPorIdAsync(instanciaEntity.EstadoAtualId.Value);
            dto.NomeEstadoAtual = estadoAtual?.Nome;
        }

        if (instanciaEntity.IniciadoPorId.HasValue)
        {
            var iniciador = await _unitOfWork.Utilizadores.ObterPorIdAsync(instanciaEntity.IniciadoPorId.Value);
            dto.NomeIniciador = iniciador?.Nome;
        }

        return dto;
    }

    private async Task ValidarMudancaStatusAsync(WorkflowInstanciaEntity instanciaEntity, StatusWorkflowEntity novoStatus)
    {
        // Validar transições de status permitidas
        switch (instanciaEntity.Status)
        {
            case StatusWorkflowEntity.Ativo:
                if (novoStatus != StatusWorkflowEntity.Suspenso && novoStatus != StatusWorkflowEntity.Cancelado)
                    throw new WorkflowInstanciaInativaException(instanciaEntity.Id);
                break;

            case StatusWorkflowEntity.Suspenso:
                if (novoStatus != StatusWorkflowEntity.Ativo && novoStatus != StatusWorkflowEntity.Cancelado)
                    throw new WorkflowInstanciaInativaException(instanciaEntity.Id);
                break;

            case StatusWorkflowEntity.Concluido:
            case StatusWorkflowEntity.Cancelado:
                throw new WorkflowInstanciaJaConcluidaException(instanciaEntity.Id);
        }

        // Verificar se o modelo ainda está ativo (para reativações)
        if (novoStatus == StatusWorkflowEntity.Ativo)
        {
            var modelo = await _unitOfWork.WorkflowModelos.ObterPorIdAsync(instanciaEntity.WorkflowModeloId);
            if (modelo == null || !modelo.Ativo)
                throw new WorkflowModeloInvalidoException(
                    $"O modelo de workflow {instanciaEntity.WorkflowModeloId} não está ativo");
        }
    }

    private void AplicarMudancaStatus(WorkflowInstanciaEntity instanciaEntity, StatusWorkflowEntity novoStatus)
    {
        switch (novoStatus)
        {
            case StatusWorkflowEntity.Ativo:
                instanciaEntity.Reativar();
                break;

            case StatusWorkflowEntity.Suspenso:
                instanciaEntity.Suspender();
                break;

            case StatusWorkflowEntity.Cancelado:
                instanciaEntity.Cancelar();
                break;

            case StatusWorkflowEntity.Concluido:
                // Conclusão só pode ocorrer via ExecutarTransicaoAsync
                throw new InvalidOperationException(
                    "Para concluir uma instância, use o método ExecutarTransicaoAsync");
        }
    }

    private async Task<string?> ObterNomeUtilizadorAsync(int utilizadorId)
    {
        var utilizador = await _unitOfWork.Utilizadores.ObterPorIdAsync(utilizadorId);
        return utilizador?.Nome;
    }

    #endregion
}