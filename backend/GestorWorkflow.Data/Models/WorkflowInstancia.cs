using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Data.Models
{
    [Table("WorkflowInstancia")]
    public class WorkflowInstancia
    {
        [Key]
        [Column("id_workflow_instancia")]
        public int WorkflowInstanciaId { get; set; }

        [Column("id_workflow_modelo")]
        public int WorkflowModeloId { get; set; }

        [Column("data_inicio")]
        public DateTime DataInicio { get; set; }

        [Column("data_fim")]
        public DateTime? DataFim { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("estado_atual_id")]
        public int? EstadoAtualId { get; set; }

        [Column("iniciado_por_utilizador_id")]
        public int? IniciadoPorUtilizadorId { get; set; }

        [Column("equipa_json")]
        public string? EquipaJson { get; set; }

        [Column("estados_concluidos_json")]
        public string? EstadosConcluidosJson { get; set; }

        // Navegação
        [ForeignKey("WorkflowModeloId")]
        public virtual WorkflowModelo? WorkflowModelo { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        [ForeignKey("EstadoAtualId")]
        public virtual EstadoModelo? EstadoAtual { get; set; }

        [ForeignKey("IniciadoPorUtilizadorId")]
        public virtual Utilizador? IniciadoPor { get; set; }

        public virtual ICollection<TransicaoInstancia> TransicoesInstancia { get; set; } = new List<TransicaoInstancia>();

        [NotMapped]
        public StatusWorkflowEntity StatusWorkflowEntity
        {
            get => (StatusWorkflowEntity)StatusId;
            set => StatusId = (int)value;
        }
    }
}