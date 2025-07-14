using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;

namespace GestorWorkflow.Core.Services;

public static class ServicesMappings
{
    public static EstadoDTO MapearParaEstadoDto(EstadoEntity estado)
    {
        return new EstadoDTO
        {
            Id = estado.Id,
            Nome = estado.Nome,
            Descricao = estado.Descricao,
            Tipo = estado.Tipo,
            CorHexadecimal = estado.CorHexadecimal,
            Ativo = estado.Ativo,
            DataCriacao = estado.DataCriacao,
            CriadoPorId = estado.CriadoPorId,
            Funcoes = estado.Funcoes ?? new List<string>(),
            PreCondicao = estado.PreCondicao,
            PosCondicao = estado.PosCondicao
        };
    }

    public static async Task<PermissaoDTO> MapearParaPermissaoDtoAsync(PermissaoEntity permissao, IUnitOfWork unitOfWork)
    {
        var dto = new PermissaoDTO
        {
            Id = permissao.Id,
            Nome = permissao.Nome,
            Descricao = permissao.Descricao,
            TransicaoId = permissao.TransicaoId
        };

        // Adicionar informações adicionais
        var transicao = await unitOfWork.Transicoes.ObterPorIdAsync(permissao.TransicaoId);
        dto.NomeTransicao = transicao?.Nome;

        return dto;
    }

    public static async Task<TransicaoDTO> MapearParaTransicaoDtoAsync(TransicaoEntity transicao, IUnitOfWork unitOfWork)
    {
        var dto = new TransicaoDTO
        {
            Id = transicao.Id,
            Nome = transicao.Nome,
            Descricao = transicao.Descricao,
            EstadoOrigemId = transicao.EstadoOrigemId,
            EstadoDestinoId = transicao.EstadoDestinoId,
            PreCondicaoId = transicao.PreCondicaoId,
            PosCondicaoId = transicao.PosCondicaoId,
            PermissoesIds = transicao.PermissoesIds
        };

        // Adicionar informações adicionais
        if (transicao.EstadoOrigemId.HasValue)
        {
            var estadoOrigem = await unitOfWork.Estados.ObterPorIdAsync(transicao.EstadoOrigemId.Value);
            dto.NomeEstadoOrigem = estadoOrigem?.Nome;
        }

        var estadoDestino = await unitOfWork.Estados.ObterPorIdAsync(transicao.EstadoDestinoId);
        dto.NomeEstadoDestino = estadoDestino?.Nome;

        if (transicao.PreCondicaoId.HasValue)
        {
            var preCondicao = await unitOfWork.PreCondicoes.ObterPorIdAsync(transicao.PreCondicaoId.Value);
            dto.NomePreCondicao = preCondicao?.Nome;
        }

        if (transicao.PosCondicaoId.HasValue)
        {
            var posCondicao = await unitOfWork.PosCondicoes.ObterPorIdAsync(transicao.PosCondicaoId.Value);
            dto.NomePosCondicao = posCondicao?.Nome;
        }

        return dto;
    }

    public static async Task<WorkflowInstanciaDTO> MapearParaWorkflowInstanciaDtoAsync(WorkflowInstanciaEntity instancia, IUnitOfWork unitOfWork)
    {
        var dto = new WorkflowInstanciaDTO
        {
            Id = instancia.Id,
            WorkflowModeloId = instancia.WorkflowModeloId,
            Status = instancia.Status,
            EstadoAtualId = instancia.EstadoAtualId,
            DataInicio = instancia.DataInicio,
            DataFim = instancia.DataFim,
            IniciadoPorId = instancia.IniciadoPorId
        };

        // Mapear histórico de transições
        foreach (var registro in instancia.HistoricoTransicoes)
            dto.HistoricoTransicoes.Add(await MapearParaRegistoTransicaoDtoAsync(registro, unitOfWork));

        // Adicionar informações adicionais
        var workflowModelo = await unitOfWork.WorkflowModelos.ObterPorIdAsync(instancia.WorkflowModeloId);
        dto.NomeWorkflowModelo = workflowModelo?.Nome;

        if (instancia.EstadoAtualId.HasValue)
        {
            var estadoAtual = await unitOfWork.Estados.ObterPorIdAsync(instancia.EstadoAtualId.Value);
            dto.NomeEstadoAtual = estadoAtual?.Nome;
        }

        if (instancia.IniciadoPorId.HasValue)
        {
            var iniciador = await unitOfWork.Utilizadores.ObterPorIdAsync(instancia.IniciadoPorId.Value);
            dto.NomeIniciador = iniciador?.Nome;
        }

        return dto;
    }

    public static async Task<RegistoTransicaoDTO> MapearParaRegistoTransicaoDtoAsync(RegistoTransicaoEntity registro, IUnitOfWork unitOfWork)
    {
        var dto = new RegistoTransicaoDTO
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
        var transicao = await unitOfWork.Transicoes.ObterPorIdAsync(registro.TransicaoId);
        dto.NomeTransicao = transicao?.Nome;

        if (registro.EstadoOrigemId.HasValue)
        {
            var estadoOrigem = await unitOfWork.Estados.ObterPorIdAsync(registro.EstadoOrigemId.Value);
            dto.NomeEstadoOrigem = estadoOrigem?.Nome;
        }

        var estadoDestino = await unitOfWork.Estados.ObterPorIdAsync(registro.EstadoDestinoId);
        dto.NomeEstadoDestino = estadoDestino?.Nome;

        if (registro.ExecutadoPorId.HasValue)
        {
            var executor = await unitOfWork.Utilizadores.ObterPorIdAsync(registro.ExecutadoPorId.Value);
            dto.NomeExecutor = executor?.Nome;
        }

        return dto;
    }
}