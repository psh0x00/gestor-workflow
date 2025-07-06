using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("WorkflowModelo")]
    public class WorkflowModelo
    {
        [Key]
        [Column("id_workflow_modelo")]
        public int WorkflowModeloId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [StringLength(500)]
        public string? Descricao { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [Column("id_estado_modelo")]
        public int? EstadoInicialId { get; set; }

        [Column("criado_por_utilizador_id")]
        public int CriadoPorUtilizadorId { get; set; }

        [Column("data_ultima_alteracao")]
        public DateTime? DataUltimaAlteracao { get; set; }

        [Column("alterado_por_utilizador_id")]
        public int? AlteradoPorUtilizadorId { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("versao")]
        [StringLength(20)]
        public string Versao { get; set; } = "1.0";

        // Navegação
        [ForeignKey("EstadoInicialId")]
        public virtual EstadoModelo? EstadoInicial { get; set; }

        [ForeignKey("CriadoPorUtilizadorId")]
        public virtual Utilizador? CriadoPor { get; set; }

        [ForeignKey("AlteradoPorUtilizadorId")]
        public virtual Utilizador? AlteradoPor { get; set; }

        public virtual ICollection<TransicaoModelo> TransicoesModelo { get; set; } = new List<TransicaoModelo>();
        public virtual ICollection<WorkflowInstancia> WorkflowInstancias { get; set; } = new List<WorkflowInstancia>();
    }
}