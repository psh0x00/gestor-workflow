using GestorWorkflow.Core.DTO;

namespace GestorWorkflow.Core.Interfaces;

public interface IPermissaoService
{
    Task<PermissaoDTO> CriarPermissaoAsync(CriarPermissaoDTO dto);
    Task<PermissaoDTO> ObterPermissaoPorIdAsync(int id);
    Task<IEnumerable<PermissaoDTO>> ObterTodasPermissoesAsync();
    Task<PermissaoDTO> AtualizarPermissaoAsync(int id, AtualizarPermissaoDTO dto);
    Task RemoverPermissaoAsync(int id);
} 