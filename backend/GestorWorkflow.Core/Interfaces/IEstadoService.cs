using GestorWorkflow.Core.DTO;

namespace GestorWorkflow.Core.Interfaces;

public interface IEstadoService
{
    Task<EstadoDTO> CriarEstadoAsync(CriarEstadoDTO dto);
    Task<EstadoDTO> ObterEstadoPorIdAsync(int id);
    Task<IEnumerable<EstadoDTO>> ObterTodosEstadosAsync();
    Task<EstadoDTO> AtualizarEstadoAsync(int id, AtualizarEstadoDTO dto);
    Task RemoverEstadoAsync(int id);
} 