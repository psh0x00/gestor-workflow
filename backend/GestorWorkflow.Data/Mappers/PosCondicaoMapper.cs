using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class PosCondicaoMapper : IMapper<PosCondicaoEntity, PosCondicao>
    {
        public PosCondicaoEntity MapToDomain(PosCondicao dataModel)
        {
            if (dataModel == null) return null;

            var posCondicao = new PosCondicaoEntity(
                dataModel.PosCondicaoId,
                dataModel.Nome,
                dataModel.CriadoPorUtilizadorId
            );

            if (!string.IsNullOrEmpty(dataModel.Descricao))
                posCondicao.AtualizarDescricao(dataModel.Descricao);

            if (!string.IsNullOrEmpty(dataModel.AcaoSql))
                posCondicao.DefinirAcao(dataModel.AcaoSql);

            if (!dataModel.Ativo)
                posCondicao.Desativar();

            return posCondicao;
        }

        public PosCondicaoEntity MapToDomainWithDetails(PosCondicao dataModel)
        {
            return MapToDomain(dataModel);
        }

        public PosCondicao MapToDataModel(PosCondicaoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new PosCondicao
            {
                // Não definimos o ID aqui pois será gerado automaticamente pelo banco de dados
                Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome)),
                Descricao = domainModel.Descricao,
                AcaoSql = domainModel.AcaoSql,
                Ativo = domainModel.Ativo,
                DataCriacao = domainModel.DataCriacao,
                CriadoPorUtilizadorId = domainModel.CriadoPorId
            };
        }

        public void MapToExistingDataModel(PosCondicaoEntity domainModel, PosCondicao existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome));
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.AcaoSql = domainModel.AcaoSql;
            existingDataModel.Ativo = domainModel.Ativo;
        }
    }
}