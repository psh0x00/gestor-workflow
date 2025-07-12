using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorWorkflow.API.Controllers;

[ApiController]
[Route("api/utilizadores")]
public class UtilizadorController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UtilizadorController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/utilizadores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilizadorDTO>>> GetAll([FromQuery] string? q = null)
    {
        var utilizadores = await _unitOfWork.Utilizadores.ObterTodosAsync();
        var lista = utilizadores
            .Where(u => string.IsNullOrEmpty(q) ||
                        u.Nome.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Select(u => new UtilizadorDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Funcao = u.Funcao,
                Email = u.Email,
                PermissoesIds = u.PermissoesIds
            })
            .ToList();
        return Ok(lista);
    }
}
