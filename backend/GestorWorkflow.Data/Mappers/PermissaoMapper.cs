using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class PermissaoMapper : IMapper<PermissaoEntity, Permissao>
    {
        public PermissaoEntity MapToDomain(Permissao dataModel)
        {
            if (dataModel == null) return null;

            var permissao = new PermissaoEntity(
                dataModel.PermissaoId,
                dataModel.Nome,
                dataModel.TransicaoModeloId
            );

            if (!string.IsNullOrEmpty(dataModel.Descricao))
                permissao.AtualizarDescricao(dataModel.Descricao);

            return permissao;
        }

        public PermissaoEntity MapToDomainWithDetails(Permissao dataModel)
        {
            return MapToDomain(dataModel);
        }

        public Permissao MapToDataModel(PermissaoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new Permissao
            {
                // Não definimos o ID aqui pois será gerado automaticamente pelo banco de dados
                Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome)),
                Descricao = domainModel.Descricao,
                TransicaoModeloId = domainModel.TransicaoId
            };
        }

        public Permissao MapToDataModel(PermissaoEntity domainModel, int workflowModeloId)
        {
            return MapToDataModel(domainModel);
        }

        public void MapToExistingDataModel(PermissaoEntity domainModel, Permissao existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome));
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.TransicaoModeloId = domainModel.TransicaoId;
        }
    }
}