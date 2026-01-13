using PropostaService.Domain.Entities.Base;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.Entities
{
    public class Proposta : Entity
    {
        public string NomeSegurado { get; private set; }
        public decimal Valor { get; private set; }
        public StatusPropostaEnum Status { get; private set; }
        public string? MotivoRecusa { get; private set; }

        protected Proposta() { }

        public Proposta(string nomeSegurado, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(nomeSegurado))
                throw new DomainException("O nome do segurado é obrigatório.");
            if (valor <= 0)
                throw new DomainException("O valor da proposta deve ser maior que zero.");

            NomeSegurado = nomeSegurado;
            Valor = valor;
            Status = StatusPropostaEnum.EmAnalise;
        }

        public void Update(string nomeSegurado, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(nomeSegurado))
                throw new DomainException("O nome do segurado é obrigatório para atualização.");
            if (valor <= 0)
                throw new DomainException("O valor da proposta deve ser maior que zero.");

            NomeSegurado = nomeSegurado;
            Valor = valor;
            RegisterUpdate();
        }

        public void Aprovar()
        {
            if (Status != StatusPropostaEnum.EmAnalise)
                throw new DomainException("Apenas propostas em análise podem ser aprovadas.");

            Status = StatusPropostaEnum.Aprovada;
            MotivoRecusa = null;
            RegisterUpdate();
        }

        public void Rejeitar(string? motivo)
        {
            if (Status != StatusPropostaEnum.EmAnalise)
                throw new DomainException("Apenas propostas em análise podem ser rejeitadas.");

            Status = StatusPropostaEnum.Rejeitada;
            MotivoRecusa = motivo;
            RegisterUpdate();
        }
    }
}