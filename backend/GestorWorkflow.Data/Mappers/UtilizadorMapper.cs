using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;
using System;

namespace GestorWorkflow.Data.Mappers
{
    public class UtilizadorMapper : IMapper<UtilizadorEntity, Utilizador>
    {
        private readonly IMapper<PermissaoEntity, Permissao> _permissaoMapper;

        public UtilizadorMapper(IMapper<PermissaoEntity, Permissao> permissaoMapper)
        {
            _permissaoMapper = permissaoMapper;
        }

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
                Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome)),
                Funcao = domainModel.Funcao
            };
        }

        public void MapToExistingDataModel(UtilizadorEntity domainModel, Utilizador existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome ?? throw new ArgumentNullException(nameof(domainModel.Nome));
            existingDataModel.Funcao = domainModel.Funcao;
        }
    }
}