using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class DeletarPropostaUseCaseTests : UseCaseTestBase
    {
        private readonly DeletarPropostaUseCase _useCase;

        public DeletarPropostaUseCaseTests()
        {
            _useCase = new DeletarPropostaUseCase(RepositoryMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarTrue_AoDeletarComSucesso()
        {
            //Arr
            var id = Guid.NewGuid();
            var proposta = new Proposta("Geremias Berdinazi", 50000m);
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(proposta);
            //Act
            var resultado = await _useCase.ExecutarAsync(id);
            //Ass
            resultado.Should().BeTrue();
            RepositoryMock.Verify(r => r.DeletarAsync(id), Times.Once);
        }

        [Fact]
        public async Task NaoDeve_Deletar_QuandoIdNaoExistir()
        {
            //Arr
            var id = Guid.NewGuid();
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Proposta?)null);
            //Act
            var act = async () => await _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>();
            RepositoryMock.Verify(r => r.DeletarAsync(id), Times.Never);
        }
    }
}