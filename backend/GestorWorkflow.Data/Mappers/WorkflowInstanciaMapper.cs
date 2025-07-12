using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;
using System.Reflection;
using System.Text.Json;

namespace GestorWorkflow.Data.Mappers
{
    public class WorkflowInstanciaMapper : IMapper<WorkflowInstanciaEntity, WorkflowInstancia>
    {
        private readonly IMapper<WorkflowModeloEntity, WorkflowModelo> _workflowModeloMapper;
        private readonly IMapper<EstadoEntity, EstadoModelo> _estadoMapper;
        private readonly IMapper<RegistoTransicaoEntity, TransicaoInstancia> _registoTransicaoMapper;

        public WorkflowInstanciaMapper(
            IMapper<WorkflowModeloEntity, WorkflowModelo> workflowModeloMapper,
            IMapper<EstadoEntity, EstadoModelo> estadoMapper,
            IMapper<RegistoTransicaoEntity, TransicaoInstancia> registoTransicaoMapper)
        {
            _workflowModeloMapper = workflowModeloMapper;
            _estadoMapper = estadoMapper;
            _registoTransicaoMapper = registoTransicaoMapper;
        }

        public WorkflowInstanciaEntity MapToDomain(WorkflowInstancia dataModel)
        {
            if (dataModel == null) return null;

            // Deserializar equipa do JSON
            List<EquipaAtribuicaoEntity> equipa = new();
            if (!string.IsNullOrEmpty(dataModel.EquipaJson))
            {
                try
                {
                    equipa = JsonSerializer.Deserialize<List<EquipaAtribuicaoEntity>>(dataModel.EquipaJson) ?? new();
                }
                catch { }
            }

            var workflowInstancia = new WorkflowInstanciaEntity(
                dataModel.WorkflowInstanciaId,
                dataModel.WorkflowModeloId,
                dataModel.EstadoAtualId ?? 0,
                dataModel.IniciadoPorUtilizadorId,
                equipa
            );

            // Atualizar Status usando reflexão (já que o setter é private)
            var statusProperty = typeof(WorkflowInstanciaEntity).GetProperty("Status");
            if (statusProperty != null)
            {
                statusProperty.SetValue(workflowInstancia, (StatusWorkflowEntity)dataModel.StatusId);
            }

            // Atualizar DataFim usando reflexão (já que o setter é private)
            var dataFimProperty = typeof(WorkflowInstanciaEntity).GetProperty("DataFim");
            if (dataFimProperty != null)
            {
                dataFimProperty.SetValue(workflowInstancia, dataModel.DataFim);
            }

            // Mapear histórico de transições se necessário
            if (dataModel.TransicoesInstancia != null && dataModel.TransicoesInstancia.Any())
            {
                var historicoField = typeof(WorkflowInstanciaEntity)
                    .GetField("_historicoTransicoes", BindingFlags.NonPublic | BindingFlags.Instance);

                if (historicoField != null)
                {
                    var historico = new List<RegistoTransicaoEntity>();

                    foreach (var transicao in dataModel.TransicoesInstancia)
                    {
                        // Obter EstadoOrigemId e EstadoDestinoId do TransicaoModelo
                        var estadoOrigemId = transicao.TransicaoModelo?.EstadoOrigemId;
                        var estadoDestinoId = transicao.TransicaoModelo?.EstadoDestinoId ?? 0;

                        var registro = new RegistoTransicaoEntity(
                            transicao.TransicaoModeloId,
                            estadoOrigemId,
                            estadoDestinoId,
                            transicao.DataExecucao,
                            transicao.ExecutadoPorUtilizadorId
                        );
                        
                        if (!transicao.Sucesso)
                            registro.MarcarComoFalha(transicao.ErroMensagem ?? "Erro desconhecido");

                        historico.Add(registro);
                    }

                    historicoField.SetValue(workflowInstancia, historico);
                }
            }

            return workflowInstancia;
        }

        public WorkflowInstanciaEntity MapToDomainWithDetails(WorkflowInstancia dataModel)
        {
            return MapToDomain(dataModel);
        }

        public WorkflowInstancia MapToDataModel(WorkflowInstanciaEntity domainModel)
        {
            if (domainModel == null) return null;

            // Serializar equipa para JSON
            string equipaJson = null;
            if (domainModel.Equipa != null && domainModel.Equipa.Count > 0)
            {
                equipaJson = JsonSerializer.Serialize(domainModel.Equipa);
            }

            return new WorkflowInstancia
            {
                // Não definimos o ID aqui pois será gerado automaticamente pelo banco de dados
                WorkflowModeloId = domainModel.WorkflowModeloId,
                StatusId = (int)domainModel.Status,
                EstadoAtualId = domainModel.EstadoAtualId,
                DataInicio = domainModel.DataInicio,
                DataFim = domainModel.DataFim,
                IniciadoPorUtilizadorId = domainModel.IniciadoPorId,
                EquipaJson = equipaJson
            };
        }

        public WorkflowInstancia MapToDataModel(WorkflowInstanciaEntity domainModel, int workflowModeloId)
        {
            return MapToDataModel(domainModel);
        }

        public void MapToExistingDataModel(WorkflowInstanciaEntity domainModel, WorkflowInstancia existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.StatusId = (int)domainModel.Status;
            existingDataModel.EstadoAtualId = domainModel.EstadoAtualId;
            existingDataModel.DataFim = domainModel.DataFim;
        }
    }
}