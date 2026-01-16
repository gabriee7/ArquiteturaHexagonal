using AutoMapper;
using ContratacaoService.Application.Contratacoes.DTOs.In;
using ContratacaoService.Application.Contratacoes.DTOs.Out;
using ContratacaoService.Application.Contratacoes.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.External.PropostaService.DTOs;
using ContratacaoService.Domain.Ports.External.PropostaService.Enums;
using ContratacaoService.Domain.Ports.Repositories;
using FluentAssertions;
using Moq;

namespace ContratacaoService.UnitTests.Application.UseCases
{
    public class CriarContratacaoUseCaseTests
    {
        private readonly Mock<IContratacaoRepository> _repositoryMock;
        private readonly Mock<IPropostaGateway> _gatewayMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CriarContratacaoUseCase _useCase;

        public CriarContratacaoUseCaseTests()
        {
            _repositoryMock = new Mock<IContratacaoRepository>();
            _gatewayMock = new Mock<IPropostaGateway>();
            _mapperMock = new Mock<IMapper>();
            _useCase = new CriarContratacaoUseCase(_repositoryMock.Object, _gatewayMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task DeveCriarContratacao_QuandoDadosForemValidos()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var input = new CriarContratacaoInput { PropostaId = propostaId };
            var propostaDto = new PropostaExternaDto { Id = propostaId, Status = StatusPropostaEnum.Aprovada };
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync((Contratacao?)null);
            _gatewayMock.Setup(g => g.ObterPorIdAsync(propostaId)).ReturnsAsync(propostaDto);
            _mapperMock.Setup(m => m.Map<ContratacaoOutput>(It.IsAny<Contratacao>())).Returns(new ContratacaoOutput { PropostaId = propostaId });
            //Act
            var result = await _useCase.ExecutarAsync(input);
            //Ass
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.CriarAsync(It.Is<Contratacao>(c => c.PropostaId == propostaId)), Times.Once);
        }

        [Fact]
        public async Task NaoDeveCriarContratacao_QuandoPropostaJaFoiContratada()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var input = new CriarContratacaoInput { PropostaId = propostaId };
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync(new Contratacao(propostaId));
            //Act
            var act = () => _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<DomainException>().WithMessage("Esta proposta já foi contratada anteriormente.");
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Contratacao>()), Times.Never);
        }

        [Fact]
        public async Task NaoDeveCriarContratacao_QuandoPropostaNaoExisteNoGateway()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var input = new CriarContratacaoInput { PropostaId = propostaId };
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync((Contratacao?)null);
            _gatewayMock.Setup(g => g.ObterPorIdAsync(propostaId)).ReturnsAsync((PropostaExternaDto?)null);
            //Act
            var act = () => _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<DomainException>().WithMessage($"Proposta {propostaId} não encontrada.");
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Contratacao>()), Times.Never);
        }

        [Theory]
        [InlineData(StatusPropostaEnum.EmAnalise)]
        [InlineData(StatusPropostaEnum.Rejeitada)]
        public async Task NaoDeveCriarContratacao_QuandoStatusDaPropostaNaoForAprovado(StatusPropostaEnum statusInvalido)
        {
            //Arr
            var propostaId = Guid.NewGuid();
            var input = new CriarContratacaoInput { PropostaId = propostaId };
            var propostaDto = new PropostaExternaDto { Id = propostaId, Status = statusInvalido };
            _repositoryMock.Setup(r => r.ObterPorPropostaIdAsync(propostaId)).ReturnsAsync((Contratacao?)null);
            _gatewayMock.Setup(g => g.ObterPorIdAsync(propostaId)).ReturnsAsync(propostaDto);
            //Act
            var act = () => _useCase.ExecutarAsync(input);
            //Ass
            await act.Should().ThrowAsync<DomainException>().WithMessage($"A proposta não pode ser contratada pois está com status: {statusInvalido}");
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Contratacao>()), Times.Never);
        }
    }
}