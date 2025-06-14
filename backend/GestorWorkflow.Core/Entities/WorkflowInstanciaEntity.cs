using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Core.Entities;

public class WorkflowInstanciaEntity
    {
        public int Id { get; private set; }
        public int WorkflowModeloId { get; private set; }
        public StatusWorkflowEntity Status { get; private set; }
        public int? EstadoAtualId { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime? DataFim { get; private set; }
        public int? IniciadoPorId { get; private set; }

        private readonly List<RegistoTransicaoEntity> _historicoTransicoes;
        public IReadOnlyList<RegistoTransicaoEntity> HistoricoTransicoes => _historicoTransicoes.AsReadOnly();

        public WorkflowInstanciaEntity(int id, int workflowModeloId, int estadoInicialId, int? iniciadoPorId = null)
        {
            Id = id;
            WorkflowModeloId = workflowModeloId;
            EstadoAtualId = estadoInicialId;
            Status = StatusWorkflowEntity.Ativo;
            DataInicio = DateTime.UtcNow;
            IniciadoPorId = iniciadoPorId;
            _historicoTransicoes = new List<RegistoTransicaoEntity>();
        }

        public void ExecutarTransicao(TransicaoEntity transicaoEntity, int? executadoPorId = null)
        {
            if (Status != StatusWorkflowEntity.Ativo)
                throw new InvalidOperationException("Não é possível executar transições em workflow inativo");

            var registro = new RegistoTransicaoEntity(
                transicaoEntity.Id,
                EstadoAtualId,
                transicaoEntity.EstadoDestinoId,
                DateTime.UtcNow,
                executadoPorId
            );

            _historicoTransicoes.Add(registro);
            EstadoAtualId = transicaoEntity.EstadoDestinoId;
        }

        public void Suspender()
        {
            if (Status == StatusWorkflowEntity.Ativo)
                Status = StatusWorkflowEntity.Suspenso;
        }

        public void Reativar()
        {
            if (Status == StatusWorkflowEntity.Suspenso)
                Status = StatusWorkflowEntity.Ativo;
        }

        public void Cancelar()
        {
            Status = StatusWorkflowEntity.Cancelado;
            DataFim = DateTime.UtcNow;
        }

        public void Concluir()
        {
            Status = StatusWorkflowEntity.Concluido;
            DataFim = DateTime.UtcNow;
        }

        public bool EstaAtivo() => Status == StatusWorkflowEntity.Ativo;
        public bool EstaConcluido() => Status == StatusWorkflowEntity.Concluido;
        public bool EstaCancelado() => Status == StatusWorkflowEntity.Cancelado;
        public bool EstaSuspenso() => Status == StatusWorkflowEntity.Suspenso;
    }