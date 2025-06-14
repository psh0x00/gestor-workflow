using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Repositories
{
    public class EstadoRepository : BaseRepository<EstadoModelo>, IEstadoRepository
    {
        private readonly IMapper<EstadoEntity, EstadoModelo> _mapper;

        public EstadoRepository(GestorWorkflowDbContext context, IMapper<EstadoEntity, EstadoModelo> mapper)
            : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<EstadoEntity?> ObterPorIdAsync(int id)
        {
            var estadoModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EstadoModeloId == id);

            return estadoModelo != null ? _mapper.MapToDomain(estadoModelo) : null;
        }

        public async Task<IEnumerable<EstadoEntity>> ObterTodosAsync()
        {
            var estadosModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .AsNoTracking()
                .ToListAsync();

            return estadosModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<EstadoEntity>> ObterAtivosAsync()
        {
            var estadosModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .Where(e => e.Ativo)
                .AsNoTracking()
                .ToListAsync();

            return estadosModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<EstadoEntity>> ObterPorTipoAsync(TipoEstadoEntity tipo)
        {
            var tipoId = (int)tipo;

            var estadosModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .Where(e => e.TipoEstadoId == tipoId)
                .AsNoTracking()
                .ToListAsync();

            return estadosModelo.Select(_mapper.MapToDomain);
        }

        public async Task<EstadoEntity> CriarAsync(EstadoEntity estadoEntity)
        {
            var estadoModelo = _mapper.MapToDataModel(estadoEntity);
            await _dbSet.AddAsync(estadoModelo);
            return _mapper.MapToDomain(estadoModelo);
        }

        public async Task<EstadoEntity> AtualizarAsync(EstadoEntity estadoEntity)
        {
            var estadoModelo = await _dbSet.FindAsync(estadoEntity.Id);
            if (estadoModelo == null)
                throw new InvalidOperationException($"Estado com ID {estadoEntity.Id} não encontrado");

            _mapper.MapToExistingDataModel(estadoEntity, estadoModelo);

            _dbSet.Update(estadoModelo);
            return _mapper.MapToDomain(estadoModelo);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.EstadoModeloId == id);
        }

        public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null)
        {
            var query = _dbSet.Where(e => e.Nome == nome);

            if (excludeId.HasValue)
                query = query.Where(e => e.EstadoModeloId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var estadoModelo = await _dbSet.FindAsync(id);
            if (estadoModelo != null)
            {
                _dbSet.Remove(estadoModelo);
            }
        }
    }
}