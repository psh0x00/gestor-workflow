using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Data.Models;
using System;
using System.Text.Json;

namespace GestorWorkflow.Data.Mappers
{
    public class EstadoMapper : IMapper<EstadoEntity, EstadoModelo>
    {
        private readonly IMapper<PermissaoEntity, Permissao> _permissaoMapper;

        public EstadoMapper(IMapper<PermissaoEntity, Permissao> permissaoMapper)
        {
            _permissaoMapper = permissaoMapper;
        }

        public EstadoEntity MapToDomain(EstadoModelo dataModel)
        {
            if (dataModel == null) return null;

            var tipoEstado = (TipoEstadoEntity)dataModel.TipoEstadoId;

            var estado = new EstadoEntity(
                dataModel.EstadoModeloId,
                dataModel.Nome ?? throw new ArgumentNullException(nameof(dataModel.Nome)),
                tipoEstado,
                dataModel.CriadoPorUtilizadorId
            );

            estado.AtualizarDescricao(dataModel.Descricao);
            estado.DefinirCor(dataModel.CorHexadecimal);
            if (!dataModel.Ativo)
            {
                estado.Desativar();
            }
            // Desserializar Funcoes
            if (!string.IsNullOrEmpty(dataModel.Funcoes))
            {
                try { estado.DefinirFuncoes(JsonSerializer.Deserialize<List<string>>(dataModel.Funcoes)); } catch { }
            }
            return estado;
        }

        public EstadoEntity MapToDomainWithDetails(EstadoModelo dataModel)
        {
            return MapToDomain(dataModel); // Não há detalhes adicionais necessários para Estado
        }

        public EstadoModelo MapToDataModel(EstadoEntity domainModel, int workflowModeloId)
        {
            if (domainModel == null) return null;

            return new EstadoModelo
            {
                // Não definimos o ID aqui pois será gerado automaticamente pelo banco de dados
                Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome)),
                Descricao = domainModel.Descricao,
                TipoEstadoId = (int)domainModel.Tipo,
                CorHexadecimal = domainModel.CorHexadecimal,
                Ativo = domainModel.Ativo,
                DataCriacao = domainModel.DataCriacao,
                CriadoPorUtilizadorId = domainModel.CriadoPorId,
                WorkflowModeloId = workflowModeloId,
                Funcoes = domainModel.Funcoes != null && domainModel.Funcoes.Count > 0 ? JsonSerializer.Serialize(domainModel.Funcoes) : null
            };
        }

        public EstadoModelo MapToDataModel(EstadoEntity domainModel)
        {
            return MapToDataModel(domainModel, 0);
        }

        public void MapToExistingDataModel(EstadoEntity domainModel, EstadoModelo dataModel)
        {
            if (domainModel == null || dataModel == null) return;

            dataModel.Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome));
            dataModel.Descricao = domainModel.Descricao;
            dataModel.TipoEstadoId = (int)domainModel.Tipo;
            dataModel.CorHexadecimal = domainModel.CorHexadecimal;
            dataModel.Ativo = domainModel.Ativo;
            // Não atualizamos DataCriacao e CriadoPorId pois são imutáveis após criação
        }
    }
}