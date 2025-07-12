namespace GestorWorkflow.Core.DTO
{
    public class EquipaAtribuicaoDTO
    {
        public string Funcao { get; set; } = string.Empty;
        public int UtilizadorId { get; set; }
        public bool? Confirmado { get; set; } // null = pendente, true = aceitou, false = recusou
    }
}
