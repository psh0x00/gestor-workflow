using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using static GestorWorkflow.Core.Services.ServicesMappings;

namespace GestorWorkflow.Core.Services;

public class TransicaoService : ITransicaoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransicaoService> _logger;

    public TransicaoService(IUnitOfWork unitOfWork, ILogger<TransicaoService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TransicaoDTO> CriarTransicaoAsync(CriarTransicaoDTO dto)
    {
        try
        {
            _logger.LogInformation("Criando nova transição");

            // Validar estado destino
            var estadoDestino = await _unitOfWork.Estados.ObterPorIdAsync(dto.EstadoDestinoId);
            if (estadoDestino == null)
                throw new EstadoNaoEncontradoException(dto.EstadoDestinoId);

            // Validar estado origem se fornecido
            if (dto.EstadoOrigemId.HasValue)
            {
                var estadoOrigem = await _unitOfWork.Estados.ObterPorIdAsync(dto.EstadoOrigemId.Value);
                if (estadoOrigem == null)
                    throw new EstadoNaoEncontradoException(dto.EstadoOrigemId.Value);
            }

            // Criar pré-condição se nome fornecido e id não
            if (!dto.PreCondicaoId.HasValue && !string.IsNullOrWhiteSpace(dto.NomePreCondicao))
            {
                var novoIdPre = (await _unitOfWork.PreCondicoes.ObterTodosAsync()).OrderByDescending(p => p.Id).FirstOrDefault()?.Id ?? 0;
                var preCondicao = new PreCondicaoEntity(novoIdPre + 1, dto.NomePreCondicao);
                var preCondicaoCriada = await _unitOfWork.PreCondicoes.CriarAsync(preCondicao);
                await _unitOfWork.SaveChangesAsync();
                dto.PreCondicaoId = preCondicaoCriada.Id;
            }
            // Validar pré-condição se fornecida
            if (dto.PreCondicaoId.HasValue)
            {
                var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(dto.PreCondicaoId.Value);
                if (preCondicao == null)
                    throw new CondicaoNaoEncontradaException(dto.PreCondicaoId.Value, "Pré-condição");
            }

            // Criar pós-condição se nome fornecido e id não
            if (!dto.PosCondicaoId.HasValue && !string.IsNullOrWhiteSpace(dto.NomePosCondicao))
            {
                var novoIdPos = (await _unitOfWork.PosCondicoes.ObterTodosAsync()).OrderByDescending(p => p.Id).FirstOrDefault()?.Id ?? 0;
                var posCondicao = new PosCondicaoEntity(novoIdPos + 1, dto.NomePosCondicao);
                var posCondicaoCriada = await _unitOfWork.PosCondicoes.CriarAsync(posCondicao);
                await _unitOfWork.SaveChangesAsync();
                dto.PosCondicaoId = posCondicaoCriada.Id;
            }
            // Validar pós-condição se fornecida
            if (dto.PosCondicaoId.HasValue)
            {
                var posCondicao = await _unitOfWork.PosCondicoes.ObterPorIdAsync(dto.PosCondicaoId.Value);
                if (posCondicao == null)
                    throw new CondicaoNaoEncontradaException(dto.PosCondicaoId.Value, "Pós-condição");
            }

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

            foreach (var permissaoId in dto.PermissoesIds)
                transicao.AdicionarPermissao(permissaoId);

            var transicaoCriada = await _unitOfWork.Transicoes.CriarAsync(transicao);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Transição criada com sucesso: ID {Id}", transicaoCriada.Id);

            return await MapearParaTransicaoDtoAsync(transicaoCriada, _unitOfWork);
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
        if (transicao == null)
            throw new TransicaoNaoEncontradaException(id);

        return await MapearParaTransicaoDtoAsync(transicao, _unitOfWork);
    }

    public async Task<IEnumerable<TransicaoDTO>> ObterTodasTransicoesAsync()
    {
        var transicoes = await _unitOfWork.Transicoes.ObterTodosAsync();
        var transicoesDTOs = new List<TransicaoDTO>();

        foreach (var transicao in transicoes)
            transicoesDTOs.Add(await MapearParaTransicaoDtoAsync(transicao, _unitOfWork));

        return transicoesDTOs;
    }

    public async Task<TransicaoDTO> AtualizarTransicaoAsync(int id, AtualizarTransicaoDTO dto)
    {
        try
        {
            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(id);
            if (transicao == null)
                throw new TransicaoNaoEncontradaException(id);

            if (!string.IsNullOrEmpty(dto.Nome))
                transicao.DefinirNome(dto.Nome);

            if (!string.IsNullOrEmpty(dto.Descricao))
                transicao.DefinirDescricao(dto.Descricao);

            if (dto.PreCondicaoId.HasValue)
            {
                var preCondicao = await _unitOfWork.PreCondicoes.ObterPorIdAsync(dto.PreCondicaoId.Value);
                if (preCondicao == null)
                    throw new CondicaoNaoEncontradaException(dto.PreCondicaoId.Value, "Pré-condição");
                transicao.DefinirPreCondicao(dto.PreCondicaoId.Value);
            }

            if (dto.PosCondicaoId.HasValue)
            {
                var posCondicao = await _unitOfWork.PosCondicoes.ObterPorIdAsync(dto.PosCondicaoId.Value);
                if (posCondicao == null)
                    throw new CondicaoNaoEncontradaException(dto.PosCondicaoId.Value, "Pós-condição");
                transicao.DefinirPosCondicao(dto.PosCondicaoId.Value);
            }

            if (dto.PermissoesIds != null)
            {
                // Remover permissões existentes
                var permissoesAtuais = transicao.PermissoesIds.ToList();
                foreach (var permissaoId in permissoesAtuais)
                    transicao.RemoverPermissao(permissaoId);

                // Adicionar novas permissões
                foreach (var permissaoId in dto.PermissoesIds)
                    transicao.AdicionarPermissao(permissaoId);
            }

            var transicaoAtualizada = await _unitOfWork.Transicoes.AtualizarAsync(transicao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaTransicaoDtoAsync(transicaoAtualizada, _unitOfWork);
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

            // Verificar se a transição está em uso
            var instancias = await _unitOfWork.WorkflowInstancias.ObterPorTransicaoAsync(id);
            if (instancias.Any())
                throw new RecursoEmUsoException("Transição", id);

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

    private async Task<int> GerarProximoIdTransicaoAsync()
    {
        var ultimaTransicao = (await _unitOfWork.Transicoes.ObterTodosAsync())
            .OrderByDescending(t => t.Id)
            .FirstOrDefault();
        return (ultimaTransicao?.Id ?? 0) + 1;
    }


} 