using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class BuscarPropostaPorIdUseCaseTests : UseCaseTestBase
    {
        private readonly BuscarPropostaPorIdUseCase _useCase;

        public BuscarPropostaPorIdUseCaseTests()
        {
            _useCase = new BuscarPropostaPorIdUseCase(RepositoryMock.Object, MapperMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarProposta_QuandoIdExistir()
        {
            //Arr
            var id = Guid.NewGuid();
            var proposta = new Proposta("Heloísa Perissé", 95000m);
            var outputEsperado = new PropostaOutput
            {
                Id = id,
                NomeSegurado = "Heloísa Perissé",
                Valor = 95000m,
                Status = "EmAnalise"
            };
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                          .ReturnsAsync(proposta);
            MapperMock.Setup(m => m.Map<PropostaOutput>(proposta))
                      .Returns(outputEsperado);
            //Act
            var resultado = await _useCase.ExecutarAsync(id);
            //Ass
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(id);
            resultado.NomeSegurado.Should().Be(proposta.NomeSegurado);
            RepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task NaoDeve_RetornarProposta_QuandoIdInexistente()
        {
            //Arr
            var id = Guid.NewGuid();
            RepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                          .ReturnsAsync((Proposta?)null);
            //Act
            var act = async () => await _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>();
            MapperMock.Verify(m => m.Map<PropostaOutput>(It.IsAny<Proposta>()), Times.Never);
        }
    }
}