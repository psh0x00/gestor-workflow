namespace GestorWorkflow.Core.Entities;

public class WorkflowModeloEntity
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public string Versao { get; private set; }
        public int EstadoInicialId { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUltimaAlteracao { get; private set; }
        public int CriadoPorId { get; private set; }
        public int? AlteradoPorId { get; private set; }

        private readonly List<TransicaoEntity> _transicoes;
        private readonly List<EstadoEntity> _estados;

        public IReadOnlyList<TransicaoEntity> Transicoes => _transicoes.AsReadOnly();
        public IReadOnlyList<EstadoEntity> Estados => _estados.AsReadOnly();

        public WorkflowModeloEntity(int id, string nome, int estadoInicialId, int criadoPorId)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));

            Id = id;
            Nome = nome;
            EstadoInicialId = estadoInicialId;
            CriadoPorId = criadoPorId;
            Versao = "1.0";
            Ativo = true;
            DataCriacao = DateTime.UtcNow;

            _transicoes = new List<TransicaoEntity>();
            _estados = new List<EstadoEntity>();
        }

        public void AdicionarEstado(EstadoEntity estadoEntity)
        {
            if (_estados.Any(e => e.Id == estadoEntity.Id))
                throw new InvalidOperationException("Estado já existe no workflow");

            _estados.Add(estadoEntity);
        }

        public void AdicionarTransicao(TransicaoEntity transicaoEntity)
        {
            if (_transicoes.Any(t => t.Id == transicaoEntity.Id))
                throw new InvalidOperationException("Transição já existe no workflow");

            _transicoes.Add(transicaoEntity);
        }

        public void AtualizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório", nameof(nome));

            Nome = nome;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void AtualizarDescricao(string? descricao)
        {
            Descricao = descricao;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void AtualizarVersao(string versao, int alteradoPorId)
        {
            if (string.IsNullOrWhiteSpace(versao))
                throw new ArgumentException("Versão é obrigatória", nameof(versao));

            Versao = versao;
            AlteradoPorId = alteradoPorId;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            Ativo = false;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            Ativo = true;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public List<TransicaoEntity> ObterTransicoesPossiveis(int estadoAtualId)
        {
            return _transicoes.Where(t => t.EstadoOrigemId == estadoAtualId).ToList();
        }

        public bool ValidarWorkflow()
        {
            // Verifica se existe pelo menos um estado inicial
            var estadosIniciais = _estados.Where(e => e.EhEstadoInicial()).ToList();
            if (!estadosIniciais.Any())
                return false;

            // Verifica se existe pelo menos um estado final
            var estadosFinais = _estados.Where(e => e.EhEstadoFinal()).ToList();
            if (!estadosFinais.Any())
                return false;

            // Verifica se o estado inicial definido existe
            if (!_estados.Any(e => e.Id == EstadoInicialId))
                return false;

            return true;
        }
    }