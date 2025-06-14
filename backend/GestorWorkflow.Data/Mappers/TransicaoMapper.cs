using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class TransicaoMapper : IMapper<TransicaoEntity, TransicaoModelo>
    {
        public TransicaoEntity MapToDomain(TransicaoModelo dataModel)
        {
            if (dataModel == null) return null;

            var transicao = new TransicaoEntity(
                dataModel.TransicaoModeloId,
                dataModel.EstadoDestinoId,
                dataModel.EstadoOrigemId
            );

            if (!string.IsNullOrEmpty(dataModel.Nome))
                transicao.DefinirNome(dataModel.Nome);

            if (!string.IsNullOrEmpty(dataModel.Descricao))
                transicao.DefinirDescricao(dataModel.Descricao);

            if (dataModel.PreCondicaoId.HasValue)
                transicao.DefinirPreCondicao(dataModel.PreCondicaoId);

            if (dataModel.PosCondicaoId.HasValue)
                transicao.DefinirPosCondicao(dataModel.PosCondicaoId);

            // Mapear permissões
            if (dataModel.Permissoes != null)
            {
                foreach (var permissao in dataModel.Permissoes)
                {
                    transicao.AdicionarPermissao(permissao.PermissaoId);
                }
            }

            return transicao;
        }

        public TransicaoEntity MapToDomainWithDetails(TransicaoModelo dataModel)
        {
            // Se houver necessidade de mapeamento com mais detalhes
            return MapToDomain(dataModel);
        }

        public TransicaoModelo MapToDataModel(TransicaoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new TransicaoModelo
            {
                TransicaoModeloId = domainModel.Id,
                Nome = domainModel.Nome,
                Descricao = domainModel.Descricao,
                EstadoOrigemId = domainModel.EstadoOrigemId,
                EstadoDestinoId = domainModel.EstadoDestinoId,
                PreCondicaoId = domainModel.PreCondicaoId,
                PosCondicaoId = domainModel.PosCondicaoId
            };
        }

        public void MapToExistingDataModel(TransicaoEntity domainModel, TransicaoModelo existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome;
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.EstadoOrigemId = domainModel.EstadoOrigemId;
            existingDataModel.EstadoDestinoId = domainModel.EstadoDestinoId;
            existingDataModel.PreCondicaoId = domainModel.PreCondicaoId;
            existingDataModel.PosCondicaoId = domainModel.PosCondicaoId;

            // Nota: As permissões são tratadas separadamente no repositório
        }
    }
}