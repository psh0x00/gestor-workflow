using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Repositories
{
    public class TransicaoRepository : BaseRepository<TransicaoModelo>, ITransicaoRepository
    {
        private readonly IMapper<TransicaoEntity, TransicaoModelo> _mapper;

        public TransicaoRepository(GestorWorkflowDbContext context, IMapper<TransicaoEntity, TransicaoModelo> mapper)
            : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransicaoEntity?> ObterPorIdAsync(int id)
        {
            var transicaoModelo = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransicaoModeloId == id);

            return transicaoModelo != null ? _mapper.MapToDomain(transicaoModelo) : null;
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterTodosAsync()
        {
            var transicoesModelo = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .AsNoTracking()
                .ToListAsync();

            return transicoesModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterPorEstadoOrigemAsync(int estadoOrigemId)
        {
            var transicoesModelo = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoOrigemId == estadoOrigemId)
                .AsNoTracking()
                .ToListAsync();

            return transicoesModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterPorEstadoDestinoAsync(int estadoDestinoId)
        {
            var transicoesModelo = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoDestinoId == estadoDestinoId)
                .AsNoTracking()
                .ToListAsync();

            return transicoesModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<TransicaoEntity>> ObterTransicoesIniciaisAsync()
        {
            var transicoesModelo = await _dbSet
                .Include(t => t.EstadoOrigem)
                .Include(t => t.EstadoDestino)
                .Include(t => t.PreCondicao)
                .Include(t => t.PosCondicao)
                .Include(t => t.Permissoes)
                .Where(t => t.EstadoOrigemId == null)
                .AsNoTracking()
                .ToListAsync();

            return transicoesModelo.Select(_mapper.MapToDomain);
        }

        public async Task<TransicaoEntity> CriarAsync(TransicaoEntity transicaoEntity)
        {
            var transicaoModelo = _mapper.MapToDataModel(transicaoEntity);
            await _dbSet.AddAsync(transicaoModelo);
            return _mapper.MapToDomain(transicaoModelo);
        }

        public async Task<TransicaoEntity> AtualizarAsync(TransicaoEntity transicaoEntity)
        {
            var transicaoModelo = await _dbSet
                .Include(t => t.Permissoes)
                .FirstOrDefaultAsync(t => t.TransicaoModeloId == transicaoEntity.Id);

            if (transicaoModelo == null)
                throw new InvalidOperationException($"Transição com ID {transicaoEntity.Id} não encontrada");

            _mapper.MapToExistingDataModel(transicaoEntity, transicaoModelo);

            // Atualizar permissões
            transicaoModelo.Permissoes.Clear();
            foreach (var permissaoId in transicaoEntity.PermissoesIds)
            {
                var permissao = await _context.Permissoes.FindAsync(permissaoId);
                if (permissao != null)
                {
                    transicaoModelo.Permissoes.Add(permissao);
                }
            }

            _dbSet.Update(transicaoModelo);
            return _mapper.MapToDomain(transicaoModelo);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _dbSet.AnyAsync(t => t.TransicaoModeloId == id);
        }

        public async Task RemoverAsync(int id)
        {
            var transicaoModelo = await _dbSet.FindAsync(id);
            if (transicaoModelo != null)
            {
                _dbSet.Remove(transicaoModelo);
            }
        }
    }
}