using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("TiposEstado")]
    public class TipoEstado
    {
        [Key]
        [Column("id_tipo_estado")]
        public int TipoEstadoId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        // Navegação
        public virtual ICollection<EstadoModelo> EstadosModelo { get; set; } = new List<EstadoModelo>();
    }
}