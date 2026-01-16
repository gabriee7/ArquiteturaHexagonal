using AutoMapper;
using ContratacaoService.Application.Common.DTOs;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports.Repositories;
using FluentAssertions;
using Moq;

namespace ContratacaoService.UnitTests.Application.UseCases
{
    public class BuscarTodasContratacoesUseCaseTests
    {
        private readonly Mock<IContratacaoRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BuscarTodasContratacoesUseCase _useCase;

        public BuscarTodasContratacoesUseCaseTests()
        {
            _repositoryMock = new Mock<IContratacaoRepository>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new BuscarTodasContratacoesUseCase(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task DeveRetornarListaPaginada_QuandoExistiremContratacoes()
        {
            //Arr
            var input = new BaseFiltroInput { Pagina = 1, TamanhoPagina = 10 };
            var contratacoes = new List<Contratacao> { new Contratacao(Guid.NewGuid()), new Contratacao(Guid.NewGuid()) };
            var totalItens = 2;
            var outputItens = new List<ContratacaoOutput> { new ContratacaoOutput(), new ContratacaoOutput() };
            _repositoryMock.Setup(r => r.ObterTodosAsync(input.Pagina, input.TamanhoPagina, input.TermoBusca, input.Ordenacao))
                .ReturnsAsync((contratacoes, totalItens));

            _mapperMock.Setup(m => m.Map<List<ContratacaoOutput>>(contratacoes))
                .Returns(outputItens);
            //Act
            var result = await _useCase.ExecutarAsync(input);
            //Ass
            result.Should().NotBeNull();
            result.Itens.Should().HaveCount(2);
            result.TotalItens.Should().Be(totalItens);
            result.PaginaAtual.Should().Be(input.Pagina);
            _repositoryMock.Verify(r => r.ObterTodosAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeveRetornarListaVazia_QuandoNaoExistiremContratacoes()
        {
            //Arr
            var input = new BaseFiltroInput { Pagina = 1, TamanhoPagina = 10 };
            var contratacoesVazias = new List<Contratacao>();
            var totalItens = 0;
            _repositoryMock.Setup(r => r.ObterTodosAsync(input.Pagina, input.TamanhoPagina, input.TermoBusca, input.Ordenacao))
                .ReturnsAsync((contratacoesVazias, totalItens));
            _mapperMock.Setup(m => m.Map<List<ContratacaoOutput>>(contratacoesVazias))
                .Returns(new List<ContratacaoOutput>());
            //Act
            var result = await _useCase.ExecutarAsync(input);
            //Ass
            result.Itens.Should().BeEmpty();
            result.TotalItens.Should().Be(0);
            result.TotalPaginas.Should().Be(0);
        }
    }
}