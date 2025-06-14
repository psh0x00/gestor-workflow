using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorWorkflow.Data.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        [Column("id_status")]
        public int StatusId { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        // Navegação
        public virtual ICollection<WorkflowInstancia> WorkflowInstancias { get; set; } = new List<WorkflowInstancia>();
    }
}