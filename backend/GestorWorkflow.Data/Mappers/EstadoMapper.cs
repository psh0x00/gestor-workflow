using GestorWorkflow.Core.Entities;
using GestorWorkflow.Data.Models;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Data.Mappers
{
    public class EstadoMapper : IMapper<EstadoEntity, EstadoModelo>
    {
        public EstadoEntity MapToDomain(EstadoModelo dataModel)
        {
            if (dataModel == null) return null;

            var tipoEstado = (TipoEstadoEntity)dataModel.TipoEstadoId;

            var estado = new EstadoEntity(
                dataModel.EstadoModeloId,
                dataModel.Nome,
                tipoEstado,
                dataModel.CriadoPorUtilizadorId
            );

            estado.AtualizarDescricao(dataModel.Descricao);
            estado.DefinirCor(dataModel.CorHexadecimal);

            if (!dataModel.Ativo)
            {
                estado.Desativar();
            }

            return estado;
        }

        public EstadoEntity MapToDomainWithDetails(EstadoModelo dataModel)
        {
            return MapToDomain(dataModel); // Não há detalhes adicionais necessários para Estado
        }

        public EstadoModelo MapToDataModel(EstadoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new EstadoModelo
            {
                EstadoModeloId = domainModel.Id,
                Nome = domainModel.Nome,
                Descricao = domainModel.Descricao,
                TipoEstadoId = (int)domainModel.Tipo,
                CorHexadecimal = domainModel.CorHexadecimal,
                Ativo = domainModel.Ativo,
                DataCriacao = domainModel.DataCriacao,
                CriadoPorUtilizadorId = domainModel.CriadoPorId
            };
        }

        public void MapToExistingDataModel(EstadoEntity domainModel, EstadoModelo dataModel)
        {
            if (domainModel == null || dataModel == null) return;

            dataModel.Nome = domainModel.Nome;
            dataModel.Descricao = domainModel.Descricao;
            dataModel.TipoEstadoId = (int)domainModel.Tipo;
            dataModel.CorHexadecimal = domainModel.CorHexadecimal;
            dataModel.Ativo = domainModel.Ativo;
            // Não atualizamos DataCriacao e CriadoPorId pois são imutáveis após criação
        }
    }
}