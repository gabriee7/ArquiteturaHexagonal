using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class CriarPropostaUseCaseTests : UseCaseTestBase
    {
        private readonly CriarPropostaUseCase _useCase;

        public CriarPropostaUseCaseTests()
        {
            _useCase = new CriarPropostaUseCase(RepositoryMock.Object, MapperMock.Object);
        }

        [Fact]
        public async Task Deve_CriarProposta_QuandoDadosForemValidos()
        {
            //Arr
            var input = new CriarPropostaInput { NomeSegurado = "Roberto Carlos", Valor = 47500m };
            var outputEsperado = new PropostaOutput { NomeSegurado = input.NomeSegurado, Valor = input.Valor };
            MapperMock.Setup(m => m.Map<PropostaOutput>(It.IsAny<Proposta>()))
                      .Returns(outputEsperado);
            //Act
            var resultado = await _useCase.ExecutarAsync(input);
            //Ass
            resultado.Should().NotBeNull();
            resultado.NomeSegurado.Should().Be(input.NomeSegurado);
            RepositoryMock.Verify(r => r.CriarAsync(It.Is<Proposta>(p =>
                p.NomeSegurado == input.NomeSegurado &&
                p.Valor == input.Valor)), Times.Once);
        }

        [Theory]
        [InlineData("", 1000)]
        [InlineData(" ", 1000)]
        [InlineData(null, 1000)]
        public async Task NaoDeve_CriarProposta_QuandoNomeForInvalido(string nomeInvalido, decimal valor)
        {
            //Arr
            var input = new CriarPropostaInput { NomeSegurado = nomeInvalido, Valor = valor };
            //Act
            var act = async () => await _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<DomainException>();
            RepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public async Task NaoDeve_CriarProposta_QuandoValorForInvalido(decimal valorInvalido)
        {
            //Arr
            var input = new CriarPropostaInput { NomeSegurado = "Marcos Jota", Valor = valorInvalido };
            //Act
            var act = async () => await _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<DomainException>();
            RepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Proposta>()), Times.Never);
        }

        [Fact]
        public async Task NaoDeve_MapearOutput_SeFalharAoPersistirNoBanco()
        {
            //Arr
            var input = new CriarPropostaInput { NomeSegurado = "Falha Banco", Valor = 500m };
            RepositoryMock.Setup(r => r.CriarAsync(It.IsAny<Proposta>()))
                          .ThrowsAsync(new Exception("Db Error"));
            //Act
            var act = async () => await _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<Exception>();
            MapperMock.Verify(m => m.Map<PropostaOutput>(It.IsAny<Proposta>()), Times.Never);
        }
    }
}