using ContratacaoService.Application.Contratacoes.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;
using FluentAssertions;
using Moq;

namespace ContratacaoService.UnitTests.Application.UseCases
{
    public class DeletarContratacaoUseCaseTests
    {
        private readonly Mock<IContratacaoRepository> _repositoryMock;
        private readonly DeletarContratacaoUseCase _useCase;

        public DeletarContratacaoUseCaseTests()
        {
            _repositoryMock = new Mock<IContratacaoRepository>();
            _useCase = new DeletarContratacaoUseCase(_repositoryMock.Object);
        }

        [Fact]
        public async Task DeveDeletarContratacao_QuandoIdExistir()
        {
            //Arr
            var contratacao = new Contratacao(Guid.NewGuid());
            var id = contratacao.Id;
            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(contratacao);
            //Act
            await _useCase.ExecutarAsync(id);
            //Ass
            _repositoryMock.Verify(r => r.DeletarAsync(id), Times.Once);
        }

        [Fact]
        public async Task NaoDeveDeletarContratacao_QuandoIdNaoExistir()
        {
            //Arr
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Contratacao?)null);
            //Act
            var act = () => _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Contratação não encontrada.");
            _repositoryMock.Verify(r => r.DeletarAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}