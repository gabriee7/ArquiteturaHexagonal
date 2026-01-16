using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class AprovarPropostaUseCaseTests : UseCaseTestBase
    {
        private readonly AprovarPropostaUseCase _useCase;

        public AprovarPropostaUseCaseTests()
        {
            _useCase = new AprovarPropostaUseCase(RepositoryMock.Object, MessageBusMock.Object);
        }

        [Fact]
        public async Task Deve_AprovarProposta_E_PublicarMensagem_QuandoDadosValidos()
        {
            //Arr
            var id = Guid.NewGuid();
            var proposta = new Proposta("Gilberto Mestrinho", 58000m);
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                          .ReturnsAsync(proposta);
            //Act
            await _useCase.ExecutarAsync(id);
            //Ass
            proposta.Status.Should().Be(StatusPropostaEnum.Aprovada);
            RepositoryMock.Verify(r => r.AtualizarAsync(proposta), Times.Once);
            MessageBusMock.Verify(m => m.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task NaoDeve_AprovarProposta_QuandoIdNaoEncontrado()
        {
            //Arr
            var id = Guid.NewGuid();
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                          .ReturnsAsync((Proposta?)null);
            //Act
            var act = async () => await _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>();
            RepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
            MessageBusMock.Verify(m => m.PublishAsync(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task NaoDeve_AprovarProposta_QuandoStatusJaForDiferenteDeEmAnalise()
        {
            //Arr
            var id = Guid.NewGuid();
            var proposta = new Proposta("Alice Soares", 78800m);
            proposta.Aprovar();
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                          .ReturnsAsync(proposta);
            //Act
            var act = async () => await _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<DomainException>()
                     .WithMessage("Apenas propostas em análise podem ser aprovadas.");
            RepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
            MessageBusMock.Verify(m => m.PublishAsync(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task NaoDeve_PublicarMensagem_SeFalharAoAtualizarNoBanco()
        {
            //Arr
            var id = Guid.NewGuid();
            var proposta = new Proposta("Cláudio Farias", 60000m);
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(proposta);
            RepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Proposta>()))
                          .ThrowsAsync(new Exception("Database connection failed"));
            //Act
            var act = async () => await _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<Exception>();
            MessageBusMock.Verify(m => m.PublishAsync(It.IsAny<object>()), Times.Never);
        }
    }
}