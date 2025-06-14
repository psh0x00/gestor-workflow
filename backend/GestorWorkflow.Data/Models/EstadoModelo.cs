using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestorWorkflow.Core.Enums;

namespace GestorWorkflow.Data.Models
{
    [Table("EstadoModelo")]
    public class EstadoModelo
    {
        [Key]
        [Column("id_estado_modelo")]
        public int EstadoModeloId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [StringLength(500)]
        public string? Descricao { get; set; }

        [Column("tipo_estado_id")]
        public int TipoEstadoId { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Column("criado_por_utilizador_id")]
        public int? CriadoPorUtilizadorId { get; set; }

        [Column("cor_hexadecimal")]
        [StringLength(7)]
        public string? CorHexadecimal { get; set; }

        // Navegação
        [ForeignKey("TipoEstadoId")]
        public virtual TipoEstado? TipoEstado { get; set; }

        [ForeignKey("CriadoPorUtilizadorId")]
        public virtual Utilizador? CriadoPor { get; set; }

        public virtual ICollection<WorkflowModelo> WorkflowModelos { get; set; } = new List<WorkflowModelo>();
        public virtual ICollection<WorkflowInstancia> WorkflowInstanciasAtivas { get; set; } = new List<WorkflowInstancia>();
        public virtual ICollection<TransicaoModelo> TransicoesModeloOrigem { get; set; } = new List<TransicaoModelo>();
        public virtual ICollection<TransicaoModelo> TransicoesModeloDestino { get; set; } = new List<TransicaoModelo>();

        [NotMapped]
        public TipoEstadoEntity TipoEstadoEntity
        {
            get => (TipoEstadoEntity)TipoEstadoId;
            set => TipoEstadoId = (int)value;
        }
    }
}