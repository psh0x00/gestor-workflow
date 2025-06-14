using GestorWorkflow.Core.DTO;

namespace GestorWorkflow.Core.Interfaces;

public interface ITransicaoService
{
    Task<TransicaoDTO> CriarTransicaoAsync(CriarTransicaoDTO dto);
    Task<TransicaoDTO> ObterTransicaoPorIdAsync(int id);
    Task<IEnumerable<TransicaoDTO>> ObterTodasTransicoesAsync();
    Task<TransicaoDTO> AtualizarTransicaoAsync(int id, AtualizarTransicaoDTO dto);
    Task RemoverTransicaoAsync(int id);
} 