using AutoMapper;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.Repositories;
using FluentAssertions;
using Moq;

namespace ContratacaoService.UnitTests.Application.UseCases
{
    public class ObterContratacaoPorIdUseCaseTests
    {
        private readonly Mock<IContratacaoRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObterContratacaoPorIdUseCase _useCase;

        public ObterContratacaoPorIdUseCaseTests()
        {
            _repositoryMock = new Mock<IContratacaoRepository>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new ObterContratacaoPorIdUseCase(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task DeveRetornarContratacao_QuandoIdExistir()
        {
            //Arr
            var contratacao = new Contratacao(Guid.NewGuid());
            var id = contratacao.Id;
            var output = new ContratacaoOutput { Id = id };
            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(contratacao);
            _mapperMock.Setup(m => m.Map<ContratacaoOutput>(contratacao)).Returns(output);
            //Act
            var result = await _useCase.ExecutarAsync(id);
            //Ass
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task NaoDeveRetornarContratacao_QuandoIdNaoExistir()
        {
            //Arr
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Contratacao?)null);
            //Act
            var act = () => _useCase.ExecutarAsync(id);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Contratação não encontrada.");
        }
    }
}