using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestorWorkflow.API.Controllers;

/// <summary>
/// Controller para gestor de modelos de workflow
/// </summary>
[ApiController]
[Route("api/workflow-modelos")]
[Authorize] // Garante que o controller só aceita requests autenticadas e preenche as claims do utilizador
public class WorkflowModeloController : ControllerBase
{
    private readonly IWorkflowModeloService _workflowModeloService;
    private readonly ILogger<WorkflowModeloController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public WorkflowModeloController(
        IWorkflowModeloService workflowModeloService,
        ILogger<WorkflowModeloController> logger,
        IUnitOfWork unitOfWork)
    {
        _workflowModeloService = workflowModeloService ?? throw new ArgumentNullException(nameof(workflowModeloService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
            // O frontend NÃO deve enviar CriadoPorId. O backend irá preencher automaticamente com o utilizador autenticado.

            // Log do JSON recebido
            _logger.LogInformation("JSON recebido em Criar: {Json}", System.Text.Json.JsonSerializer.Serialize(dto));

            // Logar todas as claims recebidas para debug
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"Claim recebida: {claim.Type} = {claim.Value}");
            }

            // Log extra: mostrar todos os claims recebidos
            Console.WriteLine("--- Claims recebidas no backend ---");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            // Obter o ID do utilizador autenticado (cobre todos os possíveis nomes de claim)
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" ||
                c.Type == "userId" ||
                c.Type == "id" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            );
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning($"Não foi possível obter o ID do utilizador autenticado. Claims disponíveis: {string.Join(", ", User.Claims.Select(c => c.Type + "=" + c.Value))}");
                return Unauthorized("Utilizador não autenticado ou sem claim de ID.");
            }

            // ATENÇÃO: O campo CriadoPorId enviado pelo frontend será SEMPRE ignorado.
            // O valor será preenchido automaticamente com o ID do utilizador autenticado (claim JWT).
            // Não é necessário (nem recomendado) enviar CriadoPorId no payload do frontend.
            dto.CriadoPorId = userId;
            if (dto.Estados != null)
            {
                foreach (var estado in dto.Estados)
                {
                    estado.CriadoPorId = userId;
                }
            }

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

    /// <summary>
    /// Endpoint de teste para criar um estado isoladamente
    /// </summary>
    /// <param name="dto">Dados do estado a ser criado</param>
    /// <returns>ID e nome do estado criado</returns>
    [HttpPost("test-estado")]
    public async Task<ActionResult> CriarEstadoTeste([FromBody] CriarEstadoDTO dto)
    {
        try
        {
            var estadoEntity = new EstadoEntity(
                0,
                dto.Nome,
                dto.Tipo,
                dto.CriadoPorId
            );
            estadoEntity.AtualizarDescricao(dto.Descricao);
            estadoEntity.DefinirCor(dto.CorHexadecimal);
            var estadoCriado = await _unitOfWork.Estados.CriarAsync(estadoEntity);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { estadoCriado.Id, estadoCriado.Nome });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar estado de teste: {Mensagem}", ex.Message);
            return StatusCode(500, ex.ToString());
        }
    }
}