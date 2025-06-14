using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class RegistroTransicaoMapper : IMapper<RegistoTransicaoEntity, TransicaoInstancia>
    {
        public RegistoTransicaoEntity MapToDomain(TransicaoInstancia dataModel)
        {
            if (dataModel == null) return null;

            var registro = new RegistoTransicaoEntity(
                dataModel.TransicaoModeloId,
                dataModel.EstadoOrigem?.EstadoModeloId,
                dataModel.EstadoDestino?.EstadoModeloId ?? 0,
                dataModel.DataExecucao,
                dataModel.ExecutadoPorUtilizadorId
            );

            if (!dataModel.Sucesso)
                registro.MarcarComoFalha(dataModel.ErroMensagem ?? "Erro desconhecido");

            return registro;
        }

        public RegistoTransicaoEntity MapToDomainWithDetails(TransicaoInstancia dataModel)
        {
            return MapToDomain(dataModel);
        }

        public TransicaoInstancia MapToDataModel(RegistoTransicaoEntity domainModel)
        {
            if (domainModel == null) return null;

            return new TransicaoInstancia
            {
                TransicaoModeloId = domainModel.TransicaoId,
                DataExecucao = domainModel.DataExecucao,
                ExecutadoPorUtilizadorId = domainModel.ExecutadoPorId,
                Sucesso = domainModel.Sucesso,
                ErroMensagem = domainModel.MensagemErro
            };
        }

        public void MapToExistingDataModel(RegistoTransicaoEntity domainModel, TransicaoInstancia existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.DataExecucao = domainModel.DataExecucao;
            existingDataModel.ExecutadoPorUtilizadorId = domainModel.ExecutadoPorId;
            existingDataModel.Sucesso = domainModel.Sucesso;
            existingDataModel.ErroMensagem = domainModel.MensagemErro;
        }
    }
}