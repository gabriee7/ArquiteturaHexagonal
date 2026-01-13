using System.ComponentModel.DataAnnotations;

namespace PropostaService.Application.Propostas.DTOs.In
{
    public class AtualizarPropostaInput
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome do segurado é obrigatório.")]
        public string NomeSegurado { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}