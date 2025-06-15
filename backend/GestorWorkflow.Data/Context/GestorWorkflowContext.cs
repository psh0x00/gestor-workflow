using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Data.Models;

namespace GestorWorkflow.Data.Context
{
    public class GestorWorkflowDbContext : DbContext
    {
        public GestorWorkflowDbContext(DbContextOptions<GestorWorkflowDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<TipoEstado> TiposEstado { get; set; }
        public DbSet<EstadoModelo> EstadosModelo { get; set; }
        public DbSet<WorkflowModelo> WorkflowModelos { get; set; }
        public DbSet<WorkflowInstancia> WorkflowInstancias { get; set; }
        public DbSet<TransicaoModelo> TransicoesModelo { get; set; }
        public DbSet<TransicaoInstancia> TransicoesInstancia { get; set; }
        public DbSet<PreCondicao> PreCondicoes { get; set; }
        public DbSet<PosCondicao> PosCondicoes { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<UtilizadorPermissao> UtilizadorPermissoes { get; set; }
        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure auto-increment for all entities
            modelBuilder.Entity<Utilizador>().Property(u => u.UtilizadorId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TipoEstado>().Property(te => te.TipoEstadoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<EstadoModelo>().Property(em => em.EstadoModeloId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WorkflowModelo>().Property(wm => wm.WorkflowModeloId).ValueGeneratedOnAdd();
            modelBuilder.Entity<WorkflowInstancia>().Property(wi => wi.WorkflowInstanciaId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TransicaoModelo>().Property(tm => tm.TransicaoModeloId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TransicaoInstancia>().Property(ti => ti.TransicaoInstanciaId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PreCondicao>().Property(pc => pc.PreCondicaoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<PosCondicao>().Property(pc => pc.PosCondicaoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Permissao>().Property(p => p.PermissaoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Status>().Property(s => s.StatusId).ValueGeneratedOnAdd();

            // Configuração da chave composta para UtilizadorPermissao
            modelBuilder.Entity<UtilizadorPermissao>()
                .HasKey(up => new { up.UtilizadorId, up.PermissaoId });

            // Configurações de relacionamentos
            ConfigureUtilizadorRelationships(modelBuilder);
            ConfigureTipoEstadoRelationships(modelBuilder);
            ConfigureEstadoModeloRelationships(modelBuilder);
            ConfigureWorkflowModeloRelationships(modelBuilder);
            ConfigureWorkflowInstanciaRelationships(modelBuilder);
            ConfigureTransicaoModeloRelationships(modelBuilder);
            ConfigureTransicaoInstanciaRelationships(modelBuilder);
            ConfigurePreCondicaoRelationships(modelBuilder);
            ConfigurePosCondicaoRelationships(modelBuilder);
            ConfigurePermissaoRelationships(modelBuilder);
            ConfigureUtilizadorPermissaoRelationships(modelBuilder);
            ConfigureStatusRelationships(modelBuilder);

            // Seed data inicial
            SeedInitialData(modelBuilder);
        }

        private void ConfigureUtilizadorRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utilizador>()
                .HasMany(u => u.UtilizadorPermissoes)
                .WithOne(up => up.Utilizador)
                .HasForeignKey(up => up.UtilizadorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Utilizador>()
                .HasMany(u => u.TransicoesExecutadas)
                .WithOne(ti => ti.ExecutadoPor)
                .HasForeignKey(ti => ti.ExecutadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigureTipoEstadoRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoEstado>()
                .HasMany(te => te.EstadosModelo)
                .WithOne(em => em.TipoEstado)
                .HasForeignKey(em => em.TipoEstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureEstadoModeloRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstadoModelo>()
                .HasOne(em => em.TipoEstado)
                .WithMany(te => te.EstadosModelo)
                .HasForeignKey(em => em.TipoEstadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstadoModelo>()
                .HasOne(em => em.CriadoPor)
                .WithMany()
                .HasForeignKey(em => em.CriadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EstadoModelo>()
                .HasMany(em => em.WorkflowModelos)
                .WithOne(wm => wm.EstadoInicial)
                .HasForeignKey(wm => wm.EstadoInicialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstadoModelo>()
                .HasMany(em => em.WorkflowInstanciasAtivas)
                .WithOne(wi => wi.EstadoAtual)
                .HasForeignKey(wi => wi.EstadoAtualId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EstadoModelo>()
                .HasMany(em => em.TransicoesModeloOrigem)
                .WithOne(tm => tm.EstadoOrigem)
                .HasForeignKey(tm => tm.EstadoOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstadoModelo>()
                .HasMany(em => em.TransicoesModeloDestino)
                .WithOne(tm => tm.EstadoDestino)
                .HasForeignKey(tm => tm.EstadoDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureWorkflowModeloRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkflowModelo>()
                .HasOne(wm => wm.EstadoInicial)
                .WithMany(em => em.WorkflowModelos)
                .HasForeignKey(wm => wm.EstadoInicialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowModelo>()
                .HasOne(wm => wm.CriadoPor)
                .WithMany()
                .HasForeignKey(wm => wm.CriadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowModelo>()
                .HasOne(wm => wm.AlteradoPor)
                .WithMany()
                .HasForeignKey(wm => wm.AlteradoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WorkflowModelo>()
                .HasMany(wm => wm.TransicoesModelo)
                .WithOne(tm => tm.WorkflowModelo)
                .HasForeignKey(tm => tm.WorkflowModeloId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkflowModelo>()
                .HasMany(wm => wm.WorkflowInstancias)
                .WithOne(wi => wi.WorkflowModelo)
                .HasForeignKey(wi => wi.WorkflowModeloId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureWorkflowInstanciaRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkflowInstancia>()
                .HasOne(wi => wi.WorkflowModelo)
                .WithMany(wm => wm.WorkflowInstancias)
                .HasForeignKey(wi => wi.WorkflowModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowInstancia>()
                .HasOne(wi => wi.Status)
                .WithMany(s => s.WorkflowInstancias)
                .HasForeignKey(wi => wi.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowInstancia>()
                .HasOne(wi => wi.EstadoAtual)
                .WithMany(em => em.WorkflowInstanciasAtivas)
                .HasForeignKey(wi => wi.EstadoAtualId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WorkflowInstancia>()
                .HasOne(wi => wi.IniciadoPor)
                .WithMany()
                .HasForeignKey(wi => wi.IniciadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WorkflowInstancia>()
                .HasMany(wi => wi.TransicoesInstancia)
                .WithOne(ti => ti.WorkflowInstancia)
                .HasForeignKey(ti => ti.WorkflowInstanciaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureTransicaoModeloRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransicaoModelo>()
                .HasOne(tm => tm.WorkflowModelo)
                .WithMany(wm => wm.TransicoesModelo)
                .HasForeignKey(tm => tm.WorkflowModeloId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransicaoModelo>()
                .HasOne(tm => tm.EstadoOrigem)
                .WithMany(em => em.TransicoesModeloOrigem)
                .HasForeignKey(tm => tm.EstadoOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransicaoModelo>()
                .HasOne(tm => tm.EstadoDestino)
                .WithMany(em => em.TransicoesModeloDestino)
                .HasForeignKey(tm => tm.EstadoDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransicaoModelo>()
                .HasOne(tm => tm.PreCondicao)
                .WithMany(pc => pc.TransicoesModelo)
                .HasForeignKey(tm => tm.PreCondicaoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TransicaoModelo>()
                .HasOne(tm => tm.PosCondicao)
                .WithMany(pc => pc.TransicoesModelo)
                .HasForeignKey(tm => tm.PosCondicaoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TransicaoModelo>()
                .HasMany(tm => tm.Permissoes)
                .WithOne(p => p.TransicaoModelo)
                .HasForeignKey(p => p.TransicaoModeloId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransicaoModelo>()
                .HasMany(tm => tm.TransicoesInstancia)
                .WithOne(ti => ti.TransicaoModelo)
                .HasForeignKey(ti => ti.TransicaoModeloId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureTransicaoInstanciaRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransicaoInstancia>()
                .HasOne(ti => ti.WorkflowInstancia)
                .WithMany(wi => wi.TransicoesInstancia)
                .HasForeignKey(ti => ti.WorkflowInstanciaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransicaoInstancia>()
                .HasOne(ti => ti.TransicaoModelo)
                .WithMany(tm => tm.TransicoesInstancia)
                .HasForeignKey(ti => ti.TransicaoModeloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransicaoInstancia>()
                .HasOne(ti => ti.ExecutadoPor)
                .WithMany(u => u.TransicoesExecutadas)
                .HasForeignKey(ti => ti.ExecutadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigurePreCondicaoRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PreCondicao>()
                .HasOne(pc => pc.CriadoPor)
                .WithMany()
                .HasForeignKey(pc => pc.CriadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PreCondicao>()
                .HasMany(pc => pc.TransicoesModelo)
                .WithOne(tm => tm.PreCondicao)
                .HasForeignKey(tm => tm.PreCondicaoId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigurePosCondicaoRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PosCondicao>()
                .HasOne(pc => pc.CriadoPor)
                .WithMany()
                .HasForeignKey(pc => pc.CriadoPorUtilizadorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PosCondicao>()
                .HasMany(pc => pc.TransicoesModelo)
                .WithOne(tm => tm.PosCondicao)
                .HasForeignKey(tm => tm.PosCondicaoId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigurePermissaoRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissao>()
                .HasOne(p => p.TransicaoModelo)
                .WithMany(tm => tm.Permissoes)
                .HasForeignKey(p => p.TransicaoModeloId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissao>()
                .HasMany(p => p.UtilizadorPermissoes)
                .WithOne(up => up.Permissao)
                .HasForeignKey(up => up.PermissaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureUtilizadorPermissaoRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UtilizadorPermissao>()
                .HasOne(up => up.Utilizador)
                .WithMany(u => u.UtilizadorPermissoes)
                .HasForeignKey(up => up.UtilizadorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UtilizadorPermissao>()
                .HasOne(up => up.Permissao)
                .WithMany(p => p.UtilizadorPermissoes)
                .HasForeignKey(up => up.PermissaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureStatusRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>()
                .HasMany(s => s.WorkflowInstancias)
                .WithOne(wi => wi.Status)
                .HasForeignKey(wi => wi.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed Status
            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, Nome = "Ativo" },
                new Status { StatusId = 2, Nome = "Pausado" },
                new Status { StatusId = 3, Nome = "Concluído" },
                new Status { StatusId = 4, Nome = "Cancelado" },
                new Status { StatusId = 5, Nome = "Pendente" }
            );

            // Seed TipoEstado
            modelBuilder.Entity<TipoEstado>().HasData(
                new TipoEstado { TipoEstadoId = 1, Nome = "Inicial" },
                new TipoEstado { TipoEstadoId = 2, Nome = "Intermediário" },
                new TipoEstado { TipoEstadoId = 3, Nome = "Final" },
                new TipoEstado { TipoEstadoId = 4, Nome = "Decisão" },
                new TipoEstado { TipoEstadoId = 5, Nome = "Aprovação" }
            );

            // Seed Utilizador Administrativo
            modelBuilder.Entity<Utilizador>().HasData(
                new Utilizador
                {
                    UtilizadorId = 1,
                    Nome = "Administrador",
                    Funcao = "Admin"
                },
                new Utilizador
                {
                    UtilizadorId = 2,
                    Nome = "Utilizador Teste",
                    Funcao = "User"
                }
            );
        }
    }
}