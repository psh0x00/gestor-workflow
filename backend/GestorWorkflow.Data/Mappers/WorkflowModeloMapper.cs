using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Data.Models;
using System.Reflection;

namespace GestorWorkflow.Data.Mappers
{
    public class WorkflowModeloMapper : IMapper<WorkflowModeloEntity, WorkflowModelo>
    {
        private readonly IMapper<EstadoEntity, EstadoModelo> _estadoMapper;
        private readonly IMapper<TransicaoEntity, TransicaoModelo> _transicaoMapper;

        public WorkflowModeloMapper(
            IMapper<EstadoEntity, EstadoModelo> estadoMapper,
            IMapper<TransicaoEntity, TransicaoModelo> transicaoMapper)
        {
            _estadoMapper = estadoMapper;
            _transicaoMapper = transicaoMapper;
        }

        public WorkflowModeloEntity MapToDomain(WorkflowModelo dataModel)
        {
            if (dataModel == null) return null;

            var workflowModelo = new WorkflowModeloEntity(
                dataModel.WorkflowModeloId,
                dataModel.Nome,
                dataModel.EstadoInicialId,
                dataModel.CriadoPorUtilizadorId
            );

            // Usar reflexão para definir propriedades private set
            SetPrivateProperty(workflowModelo, "Descricao", dataModel.Descricao);
            SetPrivateProperty(workflowModelo, "Versao", dataModel.Versao);
            SetPrivateProperty(workflowModelo, "Ativo", dataModel.Ativo);
            SetPrivateProperty(workflowModelo, "DataCriacao", dataModel.DataCriacao);
            SetPrivateProperty(workflowModelo, "DataUltimaAlteracao", dataModel.DataUltimaAlteracao);
            SetPrivateProperty(workflowModelo, "AlteradoPorId", dataModel.AlteradoPorUtilizadorId);

            // Mapear estados se existirem
            if (dataModel.TransicoesModelo != null && dataModel.TransicoesModelo.Any())
            {
                var estadosUnicos = dataModel.TransicoesModelo
                    .Select(t => t.EstadoOrigem)
                    .Concat(dataModel.TransicoesModelo.Select(t => t.EstadoDestino))
                    .Where(e => e != null)
                    .GroupBy(e => e.EstadoModeloId)
                    .Select(g => g.First())
                    .ToList();

                var estadosField = typeof(WorkflowModeloEntity)
                    .GetField("_estados", BindingFlags.NonPublic | BindingFlags.Instance);

                if (estadosField != null)
                {
                    var estadosDomain = estadosUnicos
                        .Select(e => _estadoMapper.MapToDomain(e))
                        .Where(e => e != null)
                        .ToList();

                    estadosField.SetValue(workflowModelo, estadosDomain);
                }

                // Mapear transições
                var transicoesField = typeof(WorkflowModeloEntity)
                    .GetField("_transicoes", BindingFlags.NonPublic | BindingFlags.Instance);

                if (transicoesField != null)
                {
                    var transicoesDomain = dataModel.TransicoesModelo
                        .Select(t => _transicaoMapper.MapToDomain(t))
                        .Where(t => t != null)
                        .ToList();

                    transicoesField.SetValue(workflowModelo, transicoesDomain);
                }
            }

            return workflowModelo;
        }

        public WorkflowModeloEntity MapToDomainWithDetails(WorkflowModelo dataModel)
        {
            var workflow = MapToDomain(dataModel);

            if (workflow != null && dataModel?.EstadoInicial != null)
            {
                // Se precisar de mapear o estado inicial com mais detalhes
                var estadoInicial = _estadoMapper.MapToDomainWithDetails(dataModel.EstadoInicial);

                // Usar reflexão para acessar a coleção privada de estados se necessário
                var estadosField = typeof(WorkflowModeloEntity)
                    .GetField("_estados", BindingFlags.NonPublic | BindingFlags.Instance);

                if (estadosField != null)
                {
                    var estados = (List<EstadoEntity>)estadosField.GetValue(workflow) ?? new List<EstadoEntity>();

                    // Verificar se o estado inicial já existe na lista
                    if (!estados.Any(e => e.Id == estadoInicial.Id))
                    {
                        estados.Add(estadoInicial);
                        estadosField.SetValue(workflow, estados);
                    }
                }
            }

            return workflow;
        }

        public WorkflowModelo MapToDataModel(WorkflowModeloEntity domainModel)
        {
            if (domainModel == null) return null;

            return new WorkflowModelo
            {
                WorkflowModeloId = domainModel.Id,
                Nome = domainModel.Nome,
                Descricao = domainModel.Descricao,
                Versao = domainModel.Versao,
                EstadoInicialId = domainModel.EstadoInicialId,
                Ativo = domainModel.Ativo,
                DataCriacao = domainModel.DataCriacao,
                DataUltimaAlteracao = domainModel.DataUltimaAlteracao,
                CriadoPorUtilizadorId = domainModel.CriadoPorId,
                AlteradoPorUtilizadorId = domainModel.AlteradoPorId
            };
        }

        public void MapToExistingDataModel(WorkflowModeloEntity domainModel, WorkflowModelo existingDataModel)
        {
            if (domainModel == null || existingDataModel == null) return;

            existingDataModel.Nome = domainModel.Nome;
            existingDataModel.Descricao = domainModel.Descricao;
            existingDataModel.Versao = domainModel.Versao;
            existingDataModel.Ativo = domainModel.Ativo;
            existingDataModel.DataUltimaAlteracao = domainModel.DataUltimaAlteracao;
            existingDataModel.AlteradoPorUtilizadorId = domainModel.AlteradoPorId;
        }

        private void SetPrivateProperty<T>(object obj, string propertyName, T value)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }
    }
}