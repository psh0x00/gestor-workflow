using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("Utilizador")]
    public class Utilizador
    {
        [Key]
        [Column("id_utilizador")]
        public int UtilizadorId { get; set; }

        [Required]
        [Column("Nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("Funcao")]
        [StringLength(100)]
        public string Funcao { get; set; } = string.Empty;

        // Navegação
        public virtual ICollection<UtilizadorPermissao> UtilizadorPermissoes { get; set; } = new List<UtilizadorPermissao>();
        public virtual ICollection<TransicaoInstancia> TransicoesExecutadas { get; set; } = new List<TransicaoInstancia>();
    }
}