using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using static GestorWorkflow.Core.Services.ServicesMappings;

namespace GestorWorkflow.Core.Services;

public class PermissaoService : IPermissaoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PermissaoService> _logger;

    public PermissaoService(IUnitOfWork unitOfWork, ILogger<PermissaoService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PermissaoDTO> CriarPermissaoAsync(CriarPermissaoDTO dto)
    {
        try
        {
            _logger.LogInformation("Criando nova permissão: {Nome}", dto.Nome);

            // Validar transição
            var transicao = await _unitOfWork.Transicoes.ObterPorIdAsync(dto.TransicaoId);
            if (transicao == null)
                throw new TransicaoNaoEncontradaException(dto.TransicaoId);

            // Verificar se já existe uma permissão com o mesmo nome para a transição
            if (await _unitOfWork.Permissoes.ExisteNomeAsync(dto.Nome, dto.TransicaoId))
                throw new PermissaoJaExisteException(dto.Nome, dto.TransicaoId);

            var novoId = await GerarProximoIdPermissaoAsync();
            var permissao = new PermissaoEntity(novoId, dto.Nome, dto.TransicaoId);

            if (!string.IsNullOrEmpty(dto.Descricao))
                permissao.AtualizarDescricao(dto.Descricao);

            var permissaoCriada = await _unitOfWork.Permissoes.CriarAsync(permissao);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Permissão criada com sucesso: ID {Id}", permissaoCriada.Id);

            return await MapearParaPermissaoDtoAsync(permissaoCriada, _unitOfWork);
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
        if (permissao == null)
            throw new PermissaoNaoEncontradaException(id);

        return await MapearParaPermissaoDtoAsync(permissao, _unitOfWork);
    }

    public async Task<IEnumerable<PermissaoDTO>> ObterTodasPermissoesAsync()
    {
        var permissoes = await _unitOfWork.Permissoes.ObterTodosAsync();
        var permissoesDTOs = new List<PermissaoDTO>();

        foreach (var permissao in permissoes)
            permissoesDTOs.Add(await MapearParaPermissaoDtoAsync(permissao, _unitOfWork));

        return permissoesDTOs;
    }

    public async Task<PermissaoDTO> AtualizarPermissaoAsync(int id, AtualizarPermissaoDTO dto)
    {
        try
        {
            var permissao = await _unitOfWork.Permissoes.ObterPorIdAsync(id);
            if (permissao == null)
                throw new PermissaoNaoEncontradaException(id);

            if (!string.IsNullOrEmpty(dto.Descricao))
                permissao.AtualizarDescricao(dto.Descricao);

            var permissaoAtualizada = await _unitOfWork.Permissoes.AtualizarAsync(permissao);
            await _unitOfWork.SaveChangesAsync();

            return await MapearParaPermissaoDtoAsync(permissaoAtualizada, _unitOfWork);
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

            // Verificar se a permissão está em uso
            var utilizadores = await _unitOfWork.Utilizadores.ObterComPermissaoAsync(id);
            if (utilizadores.Any())
                throw new RecursoEmUsoException("Permissão", id);

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

    private async Task<int> GerarProximoIdPermissaoAsync()
    {
        var ultimaPermissao = (await _unitOfWork.Permissoes.ObterTodosAsync())
            .OrderByDescending(p => p.Id)
            .FirstOrDefault();
        return (ultimaPermissao?.Id ?? 0) + 1;
    }


} 