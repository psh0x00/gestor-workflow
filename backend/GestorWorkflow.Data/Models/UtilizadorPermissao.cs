using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("Utilizador_Permissao")]
    public class UtilizadorPermissao
    {
        [Column("id_Utilizador")]
        public int UtilizadorId { get; set; }

        [Column("id_Permissao")]
        public int PermissaoId { get; set; }

        // Navegação
        [ForeignKey("UtilizadorId")]
        public virtual Utilizador? Utilizador { get; set; }

        [ForeignKey("PermissaoId")]
        public virtual Permissao? Permissao { get; set; }
    }
}