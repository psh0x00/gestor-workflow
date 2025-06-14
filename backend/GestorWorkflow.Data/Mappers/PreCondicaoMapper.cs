using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class PreCondicaoMapper : IMapper<PreCondicaoEntity, PreCondicao>
    {
        public PreCondicaoEntity MapToDomain(PreCondicao dataModel)
        {
            if (dataModel == null) return null;

            var preCondicao = new PreCondicaoEntity(
                dataModel.PreCondicaoId,
                dataModel.Nome,
                dataModel.CriadoPorUtilizadorId
            );

            if (!string.IsNullOrEmpty(dataModel.Descricao))
                preCondicao.AtualizarDescricao(dataModel.Descricao);

            if (!string.IsNullOrEmpty(dataModel.CondicaoSql))
                preCondicao.DefinirCondicao(dataModel.CondicaoSql);

            if (!dataModel.Ativo)
                preCondicao.Desativar();

            return preCondicao;
        }

        public PreCondicaoEntity MapToDomainWithDetails(PreCondicao dataModel)
        {
            return MapToDomain(dataModel);
        }

        public PreCondicao MapToDataModel(PreCondicaoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new PreCondicao
            {
                PreCondicaoId = domainModel.Id,
                Nome = domainModel.Nome,
                Descricao = domainModel.Descricao,
                CondicaoSql = domainModel.CondicaoSql,
                Ativo = domainModel.Ativo,
                DataCriacao = domainModel.DataCriacao,
                CriadoPorUtilizadorId = domainModel.CriadoPorId
            };
        }

        public void MapToExistingDataModel(PreCondicaoEntity domainModel, PreCondicao existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome;
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.CondicaoSql = domainModel.CondicaoSql;
            existingDataModel.Ativo = domainModel.Ativo;
        }
    }
}