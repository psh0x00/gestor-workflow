using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class UtilizadorMapper : IMapper<UtilizadorEntity, Utilizador>
    {
        public UtilizadorEntity MapToDomain(Utilizador dataModel)
        {
            if (dataModel == null) return null;

            var utilizador = new UtilizadorEntity(
                dataModel.UtilizadorId,
                dataModel.Nome,
                dataModel.Funcao
            );

            // Mapear permissões se necessário
            if (dataModel.UtilizadorPermissoes != null)
            {
                foreach (var permissao in dataModel.UtilizadorPermissoes)
                {
                    utilizador.AdicionarPermissao(permissao.PermissaoId);
                }
            }

            return utilizador;
        }

        public UtilizadorEntity MapToDomainWithDetails(Utilizador dataModel)
        {
            return MapToDomain(dataModel);
        }

        public Utilizador MapToDataModel(UtilizadorEntity domainModel)
        {
            if (domainModel == null) return null;

            return new Utilizador
            {
                UtilizadorId = domainModel.Id,
                Nome = domainModel.Nome,
                Funcao = domainModel.Funcao
            };
        }

        public void MapToExistingDataModel(UtilizadorEntity domainModel, Utilizador existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome;
            existingDataModel.Funcao = domainModel.Funcao;
        }
    }
}