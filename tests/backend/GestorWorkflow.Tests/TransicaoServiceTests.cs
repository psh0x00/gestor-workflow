using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GestorWorkflow.Core.Services;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Core.DTO;
using GestorWorkflow.Core.Exceptions;

namespace GestorWorkflow.Tests.Services
{
    public class TransicaoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<TransicaoService>> _mockLogger;
        private readonly TransicaoService _transicaoService;

        public TransicaoServiceTests()
        {
            // 1. Arrange: Setup the mocks before each test
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<TransicaoService>>();
            
            _transicaoService = new TransicaoService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CriarTransicaoAsync_DeveLancarExcecao_QuandoEstadoDestinoNaoExiste()
        {
            // Arrange
            var dto = new CriarTransicaoDTO 
            { 
                EstadoDestinoId = 999 // ID that does not exist
            };

            // Setup the mock to return null when searching for State 999
            _mockUnitOfWork.Setup(u => u.Estados.ObterPorIdAsync(999))
                           .ReturnsAsync((EstadoEntity?)null);

            // Act & Assert
            // We expect an EstadoNaoEncontradoException to be thrown
            await Assert.ThrowsAsync<EstadoNaoEncontradoException>(() => 
                _transicaoService.CriarTransicaoAsync(dto));
                
            // Verify that we never tried to save changes to the database
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ObterTransicaoPorIdAsync_DeveLancarExcecao_QuandoTransicaoNaoExiste()
        {
            // Arrange
            int invalidId = 50;
            _mockUnitOfWork.Setup(u => u.Transicoes.ObterPorIdAsync(invalidId))
                           .ReturnsAsync((TransicaoEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<TransicaoNaoEncontradaException>(() => 
                _transicaoService.ObterTransicaoPorIdAsync(invalidId));
        }
    }
}
