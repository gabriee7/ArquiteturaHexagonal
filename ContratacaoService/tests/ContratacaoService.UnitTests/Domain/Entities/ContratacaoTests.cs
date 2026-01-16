using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Exceptions;
using FluentAssertions;

namespace ContratacaoService.UnitTests.Domain.Entities
{
    public class ContratacaoTests
    {
        [Fact]
        public void DeveInstanciarContratacao_QuandoDadosForemValidos()
        {
            //Arr
            var propostaId = Guid.NewGuid();
            //Act
            var contratacao = new Contratacao(propostaId);
            //Ass
            contratacao.Should().NotBeNull();
            contratacao.PropostaId.Should().Be(propostaId);
            contratacao.Id.Should().NotBeEmpty();
            contratacao.DataContratacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            contratacao.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void NaoDeveInstanciarContratacao_QuandoPropostaIdForVazio()
        {
            //Arr
            var propostaIdVazio = Guid.Empty;
            //Act
            var act = () => new Contratacao(propostaIdVazio);
            //Ass
            act.Should().Throw<DomainException>()
                .WithMessage("O ID da proposta é obrigatório para realizar a contratação.");
        }

        [Fact]
        public void DeveMarcarComoExcluido_QuandoMetodoDeleteForChamado()
        {
            //Arr
            var contratacao = new Contratacao(Guid.NewGuid());
            //Act
            contratacao.Delete();
            //Ass
            contratacao.IsDeleted.Should().BeTrue();
            contratacao.UpdatedAt.Should().NotBeNull();
            contratacao.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}