using Moq;
using FluentAssertions;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Application.Propostas.DTOs.Out;
using PropostaService.Application.Propostas.DTOs.In;
using PropostaService.Domain.Enums;

namespace PropostaService.UnitTests.Application.UseCases
{
    public class BuscarTodasPropostasUseCaseTests : UseCaseTestBase
    {
        private readonly BuscarTodasPropostasUseCase _useCase;

        public BuscarTodasPropostasUseCaseTests()
        {
            _useCase = new BuscarTodasPropostasUseCase(RepositoryMock.Object, MapperMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarListaPaginada_QuandoDadosForemValidos()
        {
            //Arr
            var input = new BuscarPropostasInput
            {
                Pagina = 1,
                TamanhoPagina = 10,
                Status = StatusPropostaEnum.EmAnalise,
                Ordenacao = "NomeSegurado"
            };
            var propostasFake = new List<Proposta>
            {
                new Proposta("Gilberto Mestrinho", 50000m),
                new Proposta("Alice Soares", 70000m)
            };
            var totalFake = 2;
            RepositoryMock.Setup(r => r.BuscarAsync(
                input.Pagina,
                input.TamanhoPagina,
                input.TermoBusca,
                input.Status,
                input.Ordenacao))
                .ReturnsAsync((propostasFake, totalFake));
            MapperMock.Setup(m => m.Map<List<PropostaOutput>>(propostasFake))
                      .Returns(new List<PropostaOutput>
                      {
                          new PropostaOutput { NomeSegurado = "Gilberto Mestrinho" },
                          new PropostaOutput { NomeSegurado = "Alice Soares" }
                      });
            //Act
            var resultado = await _useCase.ExecutarAsync(input);
            //Ass
            resultado.Should().NotBeNull();
            resultado.Itens.Should().HaveCount(2);
            resultado.TotalItens.Should().Be(totalFake);
            RepositoryMock.Verify(r => r.BuscarAsync(
                input.Pagina,
                input.TamanhoPagina,
                input.TermoBusca,
                input.Status,
                input.Ordenacao), Times.Once);
        }

        [Fact]
        public async Task Deve_RetornarListaVazia_QuandoNaoHouverResultados()
        {
            //Arr
            var input = new BuscarPropostasInput();
            var listaEntidadesVazia = new List<Proposta>();
            var listaDtoVazia = new List<PropostaOutput>();
            RepositoryMock.Setup(r => r.BuscarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<StatusPropostaEnum?>(),
                It.IsAny<string>()))
                .ReturnsAsync((listaEntidadesVazia, 0));
            MapperMock.Setup(m => m.Map<List<PropostaOutput>>(listaEntidadesVazia))
                      .Returns(listaDtoVazia);
            //Act
            var resultado = await _useCase.ExecutarAsync(input);
            //Ass
            resultado.Itens.Should().NotBeNull();
            resultado.Itens.Should().BeEmpty();
            resultado.TotalItens.Should().Be(0);
        }
    }
}