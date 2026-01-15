namespace PropostaService.Domain.Ports.Messaging.Events
{
    public class PropostaAprovadaEvent
    {
        public Guid PropostaId { get; set; }
        public decimal Valor { get; set; }
        public string NomeSegurado { get; set; }

        public PropostaAprovadaEvent() { }

        public PropostaAprovadaEvent(Guid propostaId, decimal valor, string nomeSegurado)
        {
            PropostaId = propostaId;
            Valor = valor;
            NomeSegurado = nomeSegurado;
        }
    }
}
