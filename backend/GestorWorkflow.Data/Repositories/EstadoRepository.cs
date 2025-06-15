using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.Enums;
using GestorWorkflow.Data.Context;
using GestorWorkflow.Data.Models;
using System.Linq.Expressions;

namespace GestorWorkflow.Data.Repositories
{
    public class EstadoRepository : BaseRepository<EstadoModelo>, IEstadoRepository
    {
        private readonly IMapper<EstadoEntity, EstadoModelo> _mapper;

        public EstadoRepository(GestorWorkflowDbContext context, IMapper<EstadoEntity, EstadoModelo> mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<EstadoEntity?> ObterPorIdAsync(int id)
        {
            var estadoModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .FirstOrDefaultAsync(e => e.EstadoModeloId == id);

            return estadoModelo != null ? _mapper.MapToDomain(estadoModelo) : null;
        }

        public async Task<IEnumerable<EstadoEntity>> ObterTodosAsync()
        {
            var estadosModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .ToListAsync();

            return estadosModelo.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<EstadoEntity>> ObterAtivosAsync()
        {
            var estadosModelo = await _dbSet
                .Include(e => e.TipoEstado)
                .Include(e => e.CriadoPor)
                .Where(e => e.Ativo)
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
                .ToListAsync();

            return estadosModelo.Select(_mapper.MapToDomain);
        }

        public async Task<EstadoEntity> CriarAsync(EstadoEntity estadoEntity)
        {
            var estadoModelo = _mapper.MapToDataModel(estadoEntity);
            var createdEstado = await CreateAsync(estadoModelo);
            return _mapper.MapToDomain(createdEstado);
        }

        public async Task<EstadoEntity> AtualizarAsync(EstadoEntity estadoEntity)
        {
            var existingEstado = await _dbSet.FindAsync(estadoEntity.Id);
            if (existingEstado == null)
                throw new InvalidOperationException($"Estado with ID {estadoEntity.Id} not found.");

            _mapper.MapToExistingDataModel(estadoEntity, existingEstado);
            var updatedEstado = Update(existingEstado);
            return _mapper.MapToDomain(updatedEstado);
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
                Delete(estadoModelo);
        }
    }
}