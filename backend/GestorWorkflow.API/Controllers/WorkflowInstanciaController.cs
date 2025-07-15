using GestorWorkflow.Core.Enums;
using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using System.Linq;

namespace GestorWorkflow.API.Controllers
{
    [ApiController]
    [Route("api/workflow-instancias")]
    [Authorize]
    public class WorkflowInstanciaController : ControllerBase
    {
        // POST: api/workflow-instancias/{id}/concluir
        [HttpPost("{id}/concluir")]
        public async Task<ActionResult> ConcluirInstancia(int id)
        {
            var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
            if (db == null) return StatusCode(500, "DbContext não encontrado");
            var instancia = await db.WorkflowInstancias.FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);
            if (instancia == null)
                return NotFound();
            // Considera 2 como status 'Terminado'. Ajuste conforme enum/status real.
            instancia.StatusId = 2;
            instancia.DataFim = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Ok();
        }


        // PUT: api/workflow-instancias/{id}/estados-concluidos
        [HttpPut("{id}/estados-concluidos")]
        public async Task<IActionResult> AtualizarEstadosConcluidos(int id, [FromBody] AtualizarEstadosConcluidosDTO dto)
        {
            var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
            if (db == null) return StatusCode(500, "DbContext não encontrado");
            var instancia = await db.WorkflowInstancias.Include(w => w.WorkflowModelo).FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);
            if (instancia == null)
                return NotFound();

            // Obter utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" ||
                c.Type == "userId" ||
                c.Type == "id" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            );
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Utilizador não autenticado ou sem claim de ID.");

            // Obter equipa e função do utilizador na instância
            string? equipaJson = instancia.EquipaJson;
            string? funcoesEstado = null;
            string? funcaoUsuario = null;
            if (!string.IsNullOrEmpty(equipaJson))
            {
                try
                {
                    var equipa = JsonSerializer.Deserialize<List<GestorWorkflow.Core.DTO.EquipaAtribuicaoDTO>>(equipaJson);
                    funcaoUsuario = equipa?.FirstOrDefault(e => e.UtilizadorId == userId)?.Funcao;
                }
                catch { }
            }

            // Carregar estados do modelo
            var estadosModelo = new List<GestorWorkflow.Data.Models.EstadoModelo>();
            if (instancia.WorkflowModelo != null && db.Entry(instancia.WorkflowModelo).Collection("Estados").IsLoaded == false)
            {
                await db.Entry(instancia.WorkflowModelo).Collection("Estados").LoadAsync();
            }
            if (instancia.WorkflowModelo != null)
            {
                var estadosEnumerable = db.Entry(instancia.WorkflowModelo).Collection("Estados").CurrentValue;
                estadosModelo = estadosEnumerable != null ? estadosEnumerable.Cast<GestorWorkflow.Data.Models.EstadoModelo>().ToList() : new List<GestorWorkflow.Data.Models.EstadoModelo>();
            }

            // Estados já concluídos antes da operação
            var estadosAntes = new List<int>();
            if (!string.IsNullOrEmpty(instancia.EstadosConcluidosJson))
            {
                try { estadosAntes = JsonSerializer.Deserialize<List<int>>(instancia.EstadosConcluidosJson) ?? new List<int>(); } catch { }
            }
            var estadosNovos = (dto.EstadosConcluidos ?? new List<int>()).Except(estadosAntes).ToList();

            // Para cada estado novo, validar permissão
            foreach (var estadoId in estadosNovos)
            {
                var estadoModelo = estadosModelo.FirstOrDefault(e => e.EstadoModeloId == estadoId);
                if (estadoModelo != null)
                {
                    // Funções permitidas para o estado (JSON string)
                    List<string> funcoesPermitidas = new();
                    if (!string.IsNullOrEmpty(estadoModelo.Funcoes))
                    {
                        try { funcoesPermitidas = JsonSerializer.Deserialize<List<string>>(estadoModelo.Funcoes) ?? new List<string>(); } catch { }
                    }
                    // Se houver restrição e a função do utilizador não está permitida, negar
                    if (funcoesPermitidas.Count > 0 && (string.IsNullOrEmpty(funcaoUsuario) || !funcoesPermitidas.Contains(funcaoUsuario)))
                    {
                        return Forbid($"Utilizador não tem permissão para concluir o estado '{estadoModelo.Nome}'");
                    }
                }
            }

            instancia.EstadosConcluidosJson = JsonSerializer.Serialize(dto.EstadosConcluidos ?? new List<int>());
            await db.SaveChangesAsync();
            return Ok();
        }
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WorkflowInstanciaController> _logger;

        public WorkflowInstanciaController(IUnitOfWork unitOfWork, ILogger<WorkflowInstanciaController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/workflow-instancias/instanciados
        [HttpGet("instanciados")]
        public async Task<ActionResult<IEnumerable<WorkflowInstanciaDTO>>> ListarInstanciados([FromQuery(Name = "status_id")] int? statusId = null)
        {
            // Obter o ID do utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" ||
                c.Type == "userId" ||
                c.Type == "id" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            );
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Utilizador não autenticado ou sem claim de ID.");

            var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
            if (db == null) return StatusCode(500, "DbContext não encontrado");
            var query = db.WorkflowInstancias
                .Include(w => w.WorkflowModelo)
                .Include(w => w.EstadoAtual)
                .Include(w => w.IniciadoPor)
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(w => w.StatusId == statusId.Value);
            }

            var todas = await query.ToListAsync();
            var instanciados = new List<WorkflowInstanciaDTO>();
            foreach (var instancia in todas)
            {
                if (instancia.EquipaJson != null)
                {
                    try
                    {
                        var equipa = JsonSerializer.Deserialize<List<EquipaAtribuicaoDTO>>(instancia.EquipaJson);
                        if (equipa != null && equipa.Any(eq => eq.UtilizadorId == userId))
                        {
                            instanciados.Add(new WorkflowInstanciaDTO
                            {
                                Id = instancia.WorkflowInstanciaId,
                                WorkflowModeloId = instancia.WorkflowModeloId,
                                Status = instancia.StatusWorkflowEntity,
                                EstadoAtualId = instancia.EstadoAtualId,
                                DataInicio = instancia.DataInicio,
                                DataFim = instancia.DataFim,
                                IniciadoPorId = instancia.IniciadoPorUtilizadorId,
                                NomeWorkflowModelo = instancia.WorkflowModelo?.Nome,
                                NomeEstadoAtual = instancia.EstadoAtual?.Nome,
                                NomeIniciador = instancia.IniciadoPor?.Nome
                            });
                        }
                    }
                    catch { }
                }
            }
            return Ok(instanciados);
        }

        // GET: api/workflow-instancias/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkflowInstanciaDTO>> ObterPorId(int id)
        {
            var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
            if (db == null) return StatusCode(500, "DbContext não encontrado");

            var instancia = await db.WorkflowInstancias
                .Include(w => w.WorkflowModelo)
                .Include(w => w.EstadoAtual)
                .Include(w => w.IniciadoPor)
                .FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);

            if (instancia == null)
                return NotFound();

            var estadosConcluidos = new List<int>();
            if (!string.IsNullOrEmpty(instancia.EstadosConcluidosJson))
            {
                try
                {
                    estadosConcluidos = System.Text.Json.JsonSerializer.Deserialize<List<int>>(instancia.EstadosConcluidosJson) ?? new List<int>();
                }
                catch { }
            }

            var dto = new WorkflowInstanciaDTO
            {
                Id = instancia.WorkflowInstanciaId,
                WorkflowModeloId = instancia.WorkflowModeloId,
                Status = instancia.StatusWorkflowEntity,
                EstadoAtualId = instancia.EstadoAtualId,
                DataInicio = instancia.DataInicio,
                DataFim = instancia.DataFim,
                IniciadoPorId = instancia.IniciadoPorUtilizadorId,
                NomeWorkflowModelo = instancia.WorkflowModelo?.Nome,
                NomeEstadoAtual = instancia.EstadoAtual?.Nome,
                NomeIniciador = instancia.IniciadoPor?.Nome,
                EstadosConcluidos = estadosConcluidos,
                Equipa = !string.IsNullOrEmpty(instancia.EquipaJson) ? JsonSerializer.Deserialize<List<EquipaAtribuicaoDTO>>(instancia.EquipaJson) : null,
                EquipaJson = instancia.EquipaJson
            };

            return Ok(dto);
        }
        // ...outros métodos...

        // DTO for updating completed states
        public class AtualizarEstadosConcluidosDTO
        {
            public List<int> EstadosConcluidos { get; set; }
        }

        public class ConfirmarParticipacaoDTO

        {
            public bool Aceitar { get; set; }
        }

    // POST: api/workflow-instancias
    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarWorkflowInstanciaDTO dto)
    {
        var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
        if (db == null) return StatusCode(500, "DbContext não encontrado");

        // Validação: Verificar se o modelo existe
        var modelo = await db.WorkflowModelos.FirstOrDefaultAsync(m => m.WorkflowModeloId == dto.WorkflowModeloId);
        if (modelo == null)
            return BadRequest("Modelo de workflow não encontrado.");

        // Validação: Verificar se o estado inicial existe e pertence ao modelo
        var estadoInicial = await db.EstadosModelo.FirstOrDefaultAsync(e => e.EstadoModeloId == dto.EstadoInicialId && e.WorkflowModeloId == dto.WorkflowModeloId);
        if (estadoInicial == null)
            return BadRequest("Estado inicial inválido ou não encontrado para este modelo.");

        // Se o criador estiver na equipa, marcar como confirmado
        var equipa = dto.Equipa;
        if (dto.IniciadoPorId.HasValue && equipa != null)
        {
            var membroCriador = equipa.FirstOrDefault(e => e.UtilizadorId == dto.IniciadoPorId.Value);
            if (membroCriador != null)
                membroCriador.Confirmado = true;
        }

        var instancia = new WorkflowInstancia
        {
            WorkflowModeloId = dto.WorkflowModeloId,
            EstadoAtualId = dto.EstadoInicialId,
            DataInicio = DateTime.UtcNow,
            StatusId = 1, // 1 = Ativo (ajuste conforme enum/status real)
            IniciadoPorUtilizadorId = dto.IniciadoPorId,
            EquipaJson = equipa != null ? JsonSerializer.Serialize(equipa) : null
        };

        db.WorkflowInstancias.Add(instancia);
        await db.SaveChangesAsync();

        return Ok(new { id = instancia.WorkflowInstanciaId });
    }

    // GET: api/workflow-instancias/pendentes
    [HttpGet("pendentes")]
    public async Task<ActionResult<IEnumerable<WorkflowInstanciaDTO>>> ListarPendentes()
    {
        // Obter o ID do utilizador autenticado
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "sub" ||
            c.Type == "userId" ||
            c.Type == "id" ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        );
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized("Utilizador não autenticado ou sem claim de ID.");

        // Obter contexto real
        var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflowDbContext)) as GestorWorkflowDbContext;
        if (db == null) return StatusCode(500, "DbContext não encontrado");
        var todas = await db.WorkflowInstancias
            .Include(w => w.WorkflowModelo)
            .Include(w => w.EstadoAtual)
            .Include(w => w.IniciadoPor)
            .ToListAsync();
        var pendentes = new List<WorkflowInstanciaDTO>();
        foreach (var instancia in todas)
        {
            if (instancia.EquipaJson != null)
            {
                try
                {
                    var equipa = JsonSerializer.Deserialize<List<EquipaAtribuicaoDTO>>(instancia.EquipaJson);
                    if (equipa != null && equipa.Any(eq => eq.UtilizadorId == userId && eq.Confirmado == null))
                    {
                        pendentes.Add(new WorkflowInstanciaDTO
                        {
                            Id = instancia.WorkflowInstanciaId,
                            WorkflowModeloId = instancia.WorkflowModeloId,
                            Status = instancia.StatusWorkflowEntity,
                            EstadoAtualId = instancia.EstadoAtualId,
                            DataInicio = instancia.DataInicio,
                            DataFim = instancia.DataFim,
                            IniciadoPorId = instancia.IniciadoPorUtilizadorId,
                            NomeWorkflowModelo = instancia.WorkflowModelo?.Nome,
                            NomeEstadoAtual = instancia.EstadoAtual?.Nome,
                            NomeIniciador = instancia.IniciadoPor?.Nome
                        });
                    }
                }
                catch { }
            }
        }
        return Ok(pendentes);
    }

    // POST: api/workflow-instancias/{id}/confirmar
    [HttpPost("{id}/confirmar")]
    public async Task<ActionResult> ConfirmarParticipacao(int id, [FromBody] ConfirmarParticipacaoDTO dto)
    {
        // Obter o ID do utilizador autenticado
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "sub" ||
            c.Type == "userId" ||
            c.Type == "id" ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        );
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized("Utilizador não autenticado ou sem claim de ID.");

        // Buscar do DbContext para garantir acesso ao modelo de dados
        var db = HttpContext.RequestServices.GetService(typeof(GestorWorkflow.Data.Context.GestorWorkflowDbContext)) as GestorWorkflow.Data.Context.GestorWorkflowDbContext;
        if (db == null) return StatusCode(500, "DbContext não encontrado");
        var instancia = await db.WorkflowInstancias.FirstOrDefaultAsync(w => w.WorkflowInstanciaId == id);
        if (instancia == null)
            return NotFound();
        if (instancia.EquipaJson == null)
            return BadRequest("Equipa não definida.");
        var equipa = JsonSerializer.Deserialize<List<EquipaAtribuicaoDTO>>(instancia.EquipaJson) ?? new List<EquipaAtribuicaoDTO>();
        var membro = equipa.FirstOrDefault(e => e.UtilizadorId == userId);
        if (membro == null)
            return Forbid();

        if (!dto.Aceitar)
        {
            db.WorkflowInstancias.Remove(instancia);
            await db.SaveChangesAsync();
            return Ok(new { removido = true });
        }

        membro.Confirmado = true;
        instancia.EquipaJson = JsonSerializer.Serialize(equipa);
        await db.SaveChangesAsync();
        return Ok(new { removido = false });
    }
    }
}