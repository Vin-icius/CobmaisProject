namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa um contrato no sistema, associado a um cliente e a um tipo específico de contrato.
    /// Contém informações sobre o valor do contrato, data de vencimento e o tipo de contrato.
    /// </summary>
    public class Contrato
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime Vencimento { get; set; }
        public decimal Valor { get; set; }
        public int TipoContratoId { get; set; }
    }
}
