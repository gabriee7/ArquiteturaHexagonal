using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class RejeitarPropostaUseCaseTests : UseCaseTestBase
    {
        private readonly RejeitarPropostaUseCase _useCase;

        public RejeitarPropostaUseCaseTests()
        {
            _useCase = new RejeitarPropostaUseCase(RepositoryMock.Object);
        }

        [Fact]
        public async Task Deve_RejeitarProposta_QuandoIdValido()
        {
            //Arr
            var id = Guid.NewGuid();
            var input = new RejeitarPropostaInput { Motivo = "Restrição Serasa" };
            var proposta = new Proposta("Zé do Gado", 10000m);
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(proposta);
            //Act
            await _useCase.ExecutarAsync(id, input);
            //Ass
            proposta.Status.Should().Be(StatusPropostaEnum.Rejeitada);
            proposta.MotivoRecusa.Should().Be(input.Motivo);
            RepositoryMock.Verify(r => r.AtualizarAsync(proposta), Times.Once);
        }

        [Fact]
        public async Task NaoDeve_Rejeitar_QuandoPropostaNaoExistir()
        {
            //Arr
            var id = Guid.NewGuid();
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Proposta?)null);
            //Act
            var act = async () => await _useCase.ExecutarAsync(id, new RejeitarPropostaInput());
            //Ass
            await act.Should().ThrowAsync<NotFoundException>();
            RepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
        }
    }
}