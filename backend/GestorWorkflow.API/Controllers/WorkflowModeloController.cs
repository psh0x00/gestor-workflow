using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorWorkflow.API.Controllers;

/// <summary>
/// Controller para gestor de modelos de workflow
/// </summary>
[ApiController]
[Route("api/workflow-modelos")]
public class WorkflowModeloController : ControllerBase
{
    private readonly IWorkflowModeloService _workflowModeloService;
    private readonly ILogger<WorkflowModeloController> _logger;

    public WorkflowModeloController(
        IWorkflowModeloService workflowModeloService,
        ILogger<WorkflowModeloController> logger)
    {
        _workflowModeloService = workflowModeloService ?? throw new ArgumentNullException(nameof(workflowModeloService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Lista todos os modelos de workflow
    /// </summary>
    /// <returns>Lista de modelos de workflow</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkflowModeloDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<WorkflowModeloDTO>>> ListarTodos()
    {
        try
        {
            var modelos = await _workflowModeloService.ObterTodosWorkflowModelosAsync();
            return Ok(modelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar modelos de workflow");
            return StatusCode(500, "Erro interno ao listar modelos de workflow");
        }
    }

    /// <summary>
    /// Cria um novo modelo de workflow
    /// </summary>
    /// <param name="dto">Dados do modelo de workflow a ser criado</param>
    /// <returns>Modelo de workflow criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> Criar([FromBody] CriarWorkflowModeloDTO dto)
    {
        try
        {
            var modelo = await _workflowModeloService.CriarWorkflowModeloAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = modelo.Id }, modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar modelo de workflow");
            return StatusCode(500, "Erro interno ao criar modelo de workflow");
        }
    }

    /// <summary>
    /// Obtém um modelo de workflow pelo ID
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <returns>Modelo de workflow encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> ObterPorId(int id)
    {
        try
        {
            var modelo = await _workflowModeloService.ObterWorkflowModeloPorIdAsync(id);
            if (modelo == null)
                return NotFound($"Modelo de workflow com ID {id} não encontrado");

            return Ok(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao obter modelo de workflow");
        }
    }

    /// <summary>
    /// Atualiza um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <param name="dto">Dados atualizados do modelo de workflow</param>
    /// <returns>Modelo de workflow atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> Atualizar(int id, [FromBody] AtualizarWorkflowModeloDTO dto)
    {
        try
        {
            var modelo = await _workflowModeloService.AtualizarWorkflowModeloAsync(id, dto);
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao atualizar modelo de workflow");
        }
    }

    /// <summary>
    /// Atualiza a versão de um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <param name="dto">Dados da nova versão</param>
    /// <returns>Modelo de workflow atualizado</returns>
    [HttpPut("{id}/versao")]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> AtualizarVersao(int id, [FromBody] AtualizarWorkflowModeloDTO dto)
    {
        try
        {
            var modelo = await _workflowModeloService.AtualizarWorkflowModeloAsync(id, dto);
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar versão do modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao atualizar versão do modelo de workflow");
        }
    }

    /// <summary>
    /// Ativa um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <returns>Modelo de workflow ativado</returns>
    [HttpPost("{id}/ativar")]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> Ativar(int id)
    {
        try
        {
            var dto = new AtualizarWorkflowModeloDTO { Ativo = true };
            var modelo = await _workflowModeloService.AtualizarWorkflowModeloAsync(id, dto);
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao ativar modelo de workflow");
        }
    }

    /// <summary>
    /// Desativa um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <returns>Modelo de workflow desativado</returns>
    [HttpPost("{id}/desativar")]
    [ProducesResponseType(typeof(WorkflowModeloDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WorkflowModeloDTO>> Desativar(int id)
    {
        try
        {
            var dto = new AtualizarWorkflowModeloDTO { Ativo = false };
            var modelo = await _workflowModeloService.AtualizarWorkflowModeloAsync(id, dto);
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao desativar modelo de workflow");
        }
    }

    /// <summary>
    /// Obtém as transições de um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <returns>Lista de transições do modelo</returns>
    [HttpGet("{id}/transicoes")]
    [ProducesResponseType(typeof(IEnumerable<TransicaoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TransicaoDTO>>> ObterTransicoes(int id)
    {
        try
        {
            var modelo = await _workflowModeloService.ObterWorkflowModeloPorIdAsync(id);
            if (modelo == null)
                return NotFound($"Modelo de workflow com ID {id} não encontrado");

            return Ok(modelo.Transicoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter transições do modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao obter transições do modelo de workflow");
        }
    }

    /// <summary>
    /// Valida a estrutura de um modelo de workflow
    /// </summary>
    /// <param name="id">ID do modelo de workflow</param>
    /// <returns>Resultado da validação</returns>
    [HttpGet("{id}/validar")]
    [ProducesResponseType(typeof(ResultadoValidacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResultadoValidacaoResponse>> Validar(int id)
    {
        try
        {
            var modelo = await _workflowModeloService.ObterWorkflowModeloPorIdAsync(id);
            if (modelo == null)
                return NotFound($"Modelo de workflow com ID {id} não encontrado");

            var resultado = new ResultadoValidacaoResponse
            {
                EhValido = true
            };

            // Validar se tem estado inicial
            if (modelo.EstadoInicialId == 0)
            {
                resultado.EhValido = false;
                resultado.Erros.Add("O modelo deve ter um estado inicial definido");
            }

            // Validar se tem pelo menos uma transição
            if (!modelo.Transicoes.Any())
            {
                resultado.EhValido = false;
                resultado.Erros.Add("O modelo deve ter pelo menos uma transição");
            }

            // Validar se todas as transições têm estado de destino
            var transicoesInvalidas = modelo.Transicoes.Where(t => t.EstadoDestinoId == 0);
            foreach (var transicao in transicoesInvalidas)
            {
                resultado.EhValido = false;
                resultado.Erros.Add($"A transição {transicao.Nome} não tem estado de destino definido");
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar modelo de workflow {Id}", id);
            return StatusCode(500, "Erro interno ao validar modelo de workflow");
        }
    }
} 