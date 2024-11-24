namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa uma dívida associada a um contrato, contendo informações sobre o valor original,
    /// valor atualizado e o percentual de desconto aplicado à dívida.
    /// </summary>
    public class Divida
    {
        public Int64 Cpf { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int Contrato { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorAtualizado { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
    }
}
