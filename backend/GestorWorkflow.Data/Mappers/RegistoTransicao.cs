using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Mappers
{
    public class RegistoTransicaoMapper : IMapper<RegistoTransicaoEntity, TransicaoInstancia>
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
                DataExecucao = domainModel.DataExecucao,
                ExecutadoPorUtilizadorId = domainModel.ExecutadoPorId,
                Sucesso = domainModel.Sucesso,
                ErroMensagem = domainModel.MensagemErro
            };
        }

        public TransicaoInstancia MapToDataModel(RegistoTransicaoEntity domainModel, int workflowModeloId)
        {
            return MapToDataModel(domainModel);
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