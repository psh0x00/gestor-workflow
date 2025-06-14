using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("Permissao")]
    public class Permissao
    {
        [Key]
        [Column("id_permissao")]
        public int PermissaoId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [StringLength(500)]
        public string? Descricao { get; set; }

        [Column("id_transicao_modelo")]
        public int TransicaoModeloId { get; set; }

        // Navegação
        [ForeignKey("TransicaoModeloId")]
        public virtual TransicaoModelo? TransicaoModelo { get; set; }

        public virtual ICollection<UtilizadorPermissao> UtilizadorPermissoes { get; set; } = new List<UtilizadorPermissao>();
    }
}