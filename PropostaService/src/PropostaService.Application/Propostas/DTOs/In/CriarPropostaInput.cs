using System.ComponentModel.DataAnnotations;

namespace PropostaService.Application.Propostas.DTOs.In
{
    public class CriarPropostaInput
    {
        [Required(ErrorMessage = "O nome do segurado é obrigatório.")]
        public string NomeSegurado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}
