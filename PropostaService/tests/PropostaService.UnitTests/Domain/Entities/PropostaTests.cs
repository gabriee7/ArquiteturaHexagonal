using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;

namespace PropostaService.UnitTests.Domain.Entities
{
    public class PropostaTests
    {
        [Fact]
        public void Deve_CriarNovaProposta_ComStatusEmAnalise()
        {
            //Arr
            var nome = "João Silva";
            var valor = 1574.00m;
            //Act
            var proposta = new Proposta(nome, valor);
            //Ass
            proposta.Status.Should().Be(StatusPropostaEnum.EmAnalise);
            proposta.NomeSegurado.Should().Be(nome);
            proposta.Valor.Should().Be(valor);
        }

        [Fact]
        public void Deve_MudarStatusParaAprovada_LimpandoMotivoRecusa()
        {
            //Arr
            var proposta = new Proposta("Franscisco Batista", 1200.7m);
            //Act
            proposta.Aprovar();
            //Ass
            proposta.Status.Should().Be(StatusPropostaEnum.Aprovada);
            proposta.MotivoRecusa.Should().BeNull();
        }

        [Theory]
        [InlineData("Alto risco de fraude")]
        [InlineData(null)]
        [InlineData("")]
        public void Deve_MudarStatusParaRejeitada_MesmoComMotivoOpcional(string? motivo)
        {
            //Arr
            var proposta = new Proposta("Josefa Fernandes", 349000m);
            //Act
            proposta.Rejeitar(motivo);
            //Ass
            proposta.Status.Should().Be(StatusPropostaEnum.Rejeitada);
            proposta.MotivoRecusa.Should().Be(motivo);
        }

        [Fact]
        public void NaoDeve_AprovarProposta_SeNaoEstiverEmAnalise()
        {
            //Arr
            var proposta = new Proposta("José Ronaldo", 21200m);
            proposta.Aprovar();
            //Act
            Action act = () => proposta.Aprovar();
            //Ass
            act.Should().Throw<DomainException>()
               .WithMessage("Apenas propostas em análise podem ser aprovadas.");
        }

        [Fact]
        public void NaoDeve_RejeitarProposta_SeNaoEstiverEmAnalise()
        {
            //Arr
            var proposta = new Proposta("Maria Robertina", 74000m);
            proposta.Rejeitar("Baixo score");
            //Act
            Action act = () => proposta.Rejeitar("Baixo score");
            //Ass
            act.Should().Throw<DomainException>()
               .WithMessage("Apenas propostas em análise podem ser rejeitadas.");
        }

        [Fact]
        public void Deve_AtualizarDadosDaProposta_QuandoValidos()
        {
            //Arr
            var proposta = new Proposta("Anthonio Carlos", 120000m);
            var novoNome = "Antonio Carlos";
            var novoValor = 110000m;
            //Act
            proposta.Update(novoNome, novoValor);
            //Ass
            proposta.NomeSegurado.Should().Be(novoNome);
            proposta.Valor.Should().Be(novoValor);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void NaoDeve_AtualizarProposta_QuandoNomeSeguradoForInvalido(string nomeInvalido)
        {
            //Arr
            var proposta = new Proposta("Doroteia Silva", 62000m);
            //Act
            Action act = () => proposta.Update(nomeInvalido, 6000m);
            //Ass
            act.Should().Throw<DomainException>()
               .WithMessage("O nome do segurado é obrigatório para atualização.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void NaoDeve_AtualizarProposta_QuandoValorForMenorOuIgualAZero(decimal valorInvalido)
        {
            //Arr
            var proposta = new Proposta("David Jorde", 10000m);
            //Act
            Action act = () => proposta.Update("David Jorde", valorInvalido);
            //Ass
            act.Should().Throw<DomainException>()
               .WithMessage("O valor da proposta deve ser maior que zero.");
        }

        [Fact]
        public void Deve_MarcarComoExcluido_AoExecutarDelete()
        {
            //Arr
            var proposta = new Proposta("José Sabá", 4500m);
            //Act
            proposta.Delete();
            //Ass
            proposta.IsDeleted.Should().BeTrue();
            proposta.UpdatedAt.Should().NotBeNull();
            proposta.UpdatedAt.Value
                .Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}