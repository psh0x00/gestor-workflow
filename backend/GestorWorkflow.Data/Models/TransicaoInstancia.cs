using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("TransicaoInstancia")]
    public class TransicaoInstancia
    {
        [Key]
        [Column("id_transicao_instancia")]
        public int TransicaoInstanciaId { get; set; }

        [Column("id_workflow_instancia")]
        public int WorkflowInstanciaId { get; set; }

        [Column("id_transicao_modelo")]
        public int TransicaoModeloId { get; set; }

        [Column("data_execucao")]
        public DateTime DataExecucao { get; set; }

        [Column("executado_por_utilizador_id")]
        public int? ExecutadoPorUtilizadorId { get; set; }

        [Column("sucesso")]
        public bool Sucesso { get; set; } = true;

        [Column("erro_mensagem")]
        [StringLength(1000)]
        public string? ErroMensagem { get; set; }

        // Navegação
        [ForeignKey("WorkflowInstanciaId")]
        public virtual WorkflowInstancia? WorkflowInstancia { get; set; }

        [ForeignKey("TransicaoModeloId")]
        public virtual TransicaoModelo? TransicaoModelo { get; set; }

        [ForeignKey("ExecutadoPorUtilizadorId")]
        public virtual Utilizador? ExecutadoPor { get; set; }


        [NotMapped]
        public EstadoModelo? EstadoOrigem => TransicaoModelo?.EstadoOrigem;

        [NotMapped]
        public EstadoModelo? EstadoDestino => TransicaoModelo?.EstadoDestino;
    }
}