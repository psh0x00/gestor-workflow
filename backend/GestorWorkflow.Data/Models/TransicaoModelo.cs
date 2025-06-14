using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("TransicaoModelo")]
    public class TransicaoModelo
    {
        [Key]
        [Column("id_transicao_modelo")]
        public int TransicaoModeloId { get; set; }

        [Column("id_workflow_modelo")]
        public int WorkflowModeloId { get; set; }

        [Column("id_estado_origem")]
        public int? EstadoOrigemId { get; set; }

        [Column("id_estado_destino")]
        public int EstadoDestinoId { get; set; }

        [Column("id_pre_condicao")]
        public int? PreCondicaoId { get; set; }

        [Column("id_pos_condicao")]
        public int? PosCondicaoId { get; set; }

        [Column("nome")]
        [StringLength(100)]
        public string? Nome { get; set; }

        [Column("descricao")]
        [StringLength(500)]
        public string? Descricao { get; set; }

        // Navegação
        [ForeignKey("WorkflowModeloId")]
        public virtual WorkflowModelo? WorkflowModelo { get; set; }

        [ForeignKey("EstadoOrigemId")]
        public virtual EstadoModelo? EstadoOrigem { get; set; }

        [ForeignKey("EstadoDestinoId")]
        public virtual EstadoModelo? EstadoDestino { get; set; }

        [ForeignKey("PreCondicaoId")]
        public virtual PreCondicao? PreCondicao { get; set; }

        [ForeignKey("PosCondicaoId")]
        public virtual PosCondicao? PosCondicao { get; set; }

        public virtual ICollection<Permissao> Permissoes { get; set; } = new List<Permissao>();
        public virtual ICollection<TransicaoInstancia> TransicoesInstancia { get; set; } = new List<TransicaoInstancia>();
    }
}