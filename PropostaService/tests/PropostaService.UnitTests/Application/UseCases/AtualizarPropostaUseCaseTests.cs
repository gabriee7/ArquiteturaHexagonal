using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class AtualizarPropostaUseCaseTests : UseCaseTestBase
    {
        private readonly AtualizarPropostaUseCase _useCase;

        public AtualizarPropostaUseCaseTests()
        {
            _useCase = new AtualizarPropostaUseCase(RepositoryMock.Object, MapperMock.Object);
        }

        [Fact]
        public async Task Deve_AtualizarProposta_QuandoDadosForemValidos()
        {
            //Arr
            var input = new AtualizarPropostaInput 
            { 
                Id = Guid.NewGuid(),
                NomeSegurado = "Bruno Mezenga",
                Valor = 90000m
            };
            var propostaExistente = new Proposta("Antigo Nome", 1000m);
            RepositoryMock.Setup(r => r.ObterPorIdAsync(input.Id)).ReturnsAsync(propostaExistente);
            MapperMock.Setup(m => m.Map<PropostaOutput>(propostaExistente))
                      .Returns(new PropostaOutput { NomeSegurado = input.NomeSegurado, Valor = input.Valor });
            //Act
            var resultado = await _useCase.ExecutarAsync(input);
            //Ass
            propostaExistente.NomeSegurado.Should().Be(input.NomeSegurado);
            RepositoryMock.Verify(r => r.AtualizarAsync(propostaExistente), Times.Once);
            resultado.NomeSegurado.Should().Be(input.NomeSegurado);
        }

        [Fact]
        public async Task NaoDeve_Atualizar_QuandoPropostaNaoExistir()
        {
            //Arr
            var input = new AtualizarPropostaInput { Id = Guid.NewGuid() };
            RepositoryMock.Setup(r => r.ObterPorIdAsync(input.Id)).ReturnsAsync((Proposta?)null);
            //Act
            var act = async () => await _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>();
            RepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Proposta>()), Times.Never);
        }
    }
}