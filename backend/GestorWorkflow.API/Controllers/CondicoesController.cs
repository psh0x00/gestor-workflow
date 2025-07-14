
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace GestorWorkflow.API.Controllers
{
    public class CriarCondicaoDTO
    {
        public string Nome { get; set; }
        public string Tipo { get; set; } // "pre" ou "pos"
    }

    [ApiController]
    [Route("api/condicoes")]
    public class CondicoesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CondicoesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var pre = await _unitOfWork.PreCondicoes.ObterTodosAsync();
            var pos = await _unitOfWork.PosCondicoes.ObterTodosAsync();
            var todas = pre.Cast<object>().Concat(pos.Cast<object>()).ToList();
            return Ok(todas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarCondicaoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Tipo))
                return BadRequest("Nome e Tipo são obrigatórios.");

            if (dto.Tipo.ToLower() == "pre")
            {
                var pre = new PreCondicaoEntity(0, dto.Nome);
                var created = await _unitOfWork.PreCondicoes.CriarAsync(pre);
                await _unitOfWork.SaveChangesAsync();
                return Ok(created);
            }
            else if (dto.Tipo.ToLower() == "pos")
            {
                var pos = new PosCondicaoEntity(0, dto.Nome);
                var created = await _unitOfWork.PosCondicoes.CriarAsync(pos);
                await _unitOfWork.SaveChangesAsync();
                return Ok(created);
            }
            else
            {
                return BadRequest("Tipo deve ser 'pre' ou 'pos'.");
            }
        }
    }
}
