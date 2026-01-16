namespace ContratacaoService.Infrastructure.Adapters.Messaging.Events
{
    public class PropostaAprovadaEvent
    {
        public Guid PropostaId { get; set; }

        public decimal Valor { get; set; }

        public string NomeSegurado { get; set; }

        public PropostaAprovadaEvent() { }
    }
}
