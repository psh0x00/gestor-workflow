using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("PosCondicao")]
    public class PosCondicao
    {
        [Key]
        [Column("id_poscondicao")]
        public int PosCondicaoId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [StringLength(500)]
        public string? Descricao { get; set; }

        [Column("acao_sql")]
        [StringLength(1000)]
        public string? AcaoSql { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Column("criado_por_utilizador_id")]
        public int? CriadoPorUtilizadorId { get; set; }

        // Navegação
        [ForeignKey("CriadoPorUtilizadorId")]
        public virtual Utilizador? CriadoPor { get; set; }

        public virtual ICollection<TransicaoModelo> TransicoesModelo { get; set; } = new List<TransicaoModelo>();
    }
}