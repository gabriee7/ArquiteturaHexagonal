using ContratacaoService.Domain.Entities.Base;
using ContratacaoService.Domain.Exceptions;

namespace ContratacaoService.Domain.Entities
{
    public class Contratacao : Entity
    {
        public Guid PropostaId { get; private set; }
        public DateTime DataContratacao { get; private set; }

        protected Contratacao() { }

        public Contratacao(Guid propostaId)
        {
            if (propostaId == Guid.Empty)
                throw new DomainException("O ID da proposta é obrigatório para realizar a contratação.");
            PropostaId = propostaId;
            DataContratacao = DateTime.UtcNow;
        }
    }
}
