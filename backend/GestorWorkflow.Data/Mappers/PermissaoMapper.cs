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
                PermissaoId = domainModel.Id,
                Nome = domainModel.Nome,
                Descricao = domainModel.Descricao,
                TransicaoModeloId = domainModel.TransicaoId
            };
        }

        public void MapToExistingDataModel(PermissaoEntity domainModel, Permissao existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome;
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.TransicaoModeloId = domainModel.TransicaoId;
        }
    }
}