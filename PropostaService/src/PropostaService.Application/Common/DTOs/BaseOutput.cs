namespace PropostaService.Application.Common.DTOs
{
    public abstract class BaseOutput
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
