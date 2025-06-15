using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories
{
    public class TransicaoRepository : BaseRepository<TransicaoModelo>, ITransicaoRepository
    {
        private readonly IMapper<TransicaoEntity, TransicaoModelo> _mapper;

        public TransicaoRepository(GestorWorkflowDbContext context, IMapper<TransicaoEntity, TransicaoModelo> mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<TransicaoEntity?> ObterPorIdAsync(int id)
        {
            var transicao = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .FirstOrDefaultAsync(t => t.TransicaoModeloId == id);

            return transicao != null ? _mapper.MapToDomain(transicao) : null;
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterTodosAsync()
        {
            var transicoes = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .ToListAsync();

            return transicoes.Select(t => _mapper.MapToDomain(t));
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterPorEstadoOrigemAsync(int estadoOrigemId)
        {
            var transicoes = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoOrigemId == estadoOrigemId)
                .ToListAsync();

            return transicoes.Select(t => _mapper.MapToDomain(t));
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterPorEstadoDestinoAsync(int estadoDestinoId)
        {
            var transicoes = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoDestinoId == estadoDestinoId)
                .ToListAsync();

            return transicoes.Select(t => _mapper.MapToDomain(t));
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterTransicoesIniciaisAsync()
        {
            var transicoes = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoOrigemId == null)
                .ToListAsync();

            return transicoes.Select(t => _mapper.MapToDomain(t));
        }

        public async Task<TransicaoEntity> CriarAsync(TransicaoEntity transicaoEntity)
        {
            var transicao = _mapper.MapToDataModel(transicaoEntity);
            var createdTransicao = await CreateAsync(transicao);
            return _mapper.MapToDomain(createdTransicao);
        }

        public async Task<TransicaoEntity> AtualizarAsync(TransicaoEntity transicaoEntity)
        {
            var existingTransicao = await _dbSet.FindAsync(transicaoEntity.Id);
            if (existingTransicao == null)
                throw new InvalidOperationException($"Transicao with ID {transicaoEntity.Id} not found.");

            _mapper.MapToExistingDataModel(transicaoEntity, existingTransicao);
            var updatedTransicao = Update(existingTransicao);
            return _mapper.MapToDomain(updatedTransicao);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _dbSet.AnyAsync(t => t.TransicaoModeloId == id);
        }

        public async Task RemoverAsync(int id)
        {
            var transicao = await _dbSet.FindAsync(id);
            if (transicao != null)
                Delete(transicao);
        }
    }
}