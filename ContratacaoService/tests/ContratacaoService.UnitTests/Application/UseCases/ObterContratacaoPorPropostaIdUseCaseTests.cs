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
    public class ObterContratacaoPorPropostaIdUseCaseTests
    {
        private readonly Mock<IContratacaoRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ObterContratacaoPorPropostaIdUseCase _useCase;

        public ObterContratacaoPorPropostaIdUseCaseTests()
        {
            _repositoryMock = new Mock<IContratacaoRepository>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new ObterContratacaoPorPropostaIdUseCase(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task DeveRetornarContratacao_QuandoPropostaIdExistir()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var contratacao = new Contratacao(propostaId);
            var output = new ContratacaoOutput { PropostaId = propostaId };
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync(contratacao);
            _mapperMock.Setup(m => m.Map<ContratacaoOutput>(contratacao)).Returns(output);
            //Act
            var result = await _useCase.ExecutarAsync(propostaId);
            //Ass
            result.Should().NotBeNull();
            result!.PropostaId.Should().Be(propostaId);
        }

        [Fact]
        public async Task NaoDeveRetornarContratacao_QuandoPropostaIdNaoExistir()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync((Contratacao?)null);
            //Act
            var act = () => _useCase.ExecutarAsync(propostaId);
            //Ass
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Contratação não encontrada.");
        }
    }
}