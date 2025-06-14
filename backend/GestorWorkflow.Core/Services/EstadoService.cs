using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Exceptions;
using GestorWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestorWorkflow.Core.Services;

public class EstadoService : IEstadoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EstadoService> _logger;

    public EstadoService(IUnitOfWork unitOfWork, ILogger<EstadoService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EstadoDTO> CriarEstadoAsync(CriarEstadoDTO dto)
    {
        try
        {
            _logger.LogInformation("Criando novo estado: {Nome}", dto.Nome);

            if (await _unitOfWork.Estados.ExisteNomeAsync(dto.Nome))
                throw new EstadoNomeJaExisteException(dto.Nome);

            var novoId = await GerarProximoIdEstadoAsync();
            var estado = new EstadoEntity(novoId, dto.Nome, dto.Tipo, dto.CriadoPorId);

            if (!string.IsNullOrEmpty(dto.Descricao))
                estado.AtualizarDescricao(dto.Descricao);

            if (!string.IsNullOrEmpty(dto.CorHexadecimal))
                estado.DefinirCor(dto.CorHexadecimal);

            var estadoCriado = await _unitOfWork.Estados.CriarAsync(estado);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Estado criado com sucesso: ID {Id}", estadoCriado.Id);

            return ServicesMappings.MapearParaEstadoDto(estadoCriado);
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
        if (estado == null)
            throw new EstadoNaoEncontradoException(id);

        return ServicesMappings.MapearParaEstadoDto(estado);
    }

    public async Task<IEnumerable<EstadoDTO>> ObterTodosEstadosAsync()
    {
        var estados = await _unitOfWork.Estados.ObterTodosAsync();
        return estados.Select(ServicesMappings.MapearParaEstadoDto);
    }

    public async Task<EstadoDTO> AtualizarEstadoAsync(int id, AtualizarEstadoDTO dto)
    {
        try
        {
            var estado = await _unitOfWork.Estados.ObterPorIdAsync(id);
            if (estado == null)
                throw new EstadoNaoEncontradoException(id);

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

            return ServicesMappings.MapearParaEstadoDto(estadoAtualizado);
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

    private async Task<int> GerarProximoIdEstadoAsync()
    {
        var ultimoEstado = (await _unitOfWork.Estados.ObterTodosAsync()).OrderByDescending(e => e.Id).FirstOrDefault();
        return (ultimoEstado?.Id ?? 0) + 1;
    }
} 