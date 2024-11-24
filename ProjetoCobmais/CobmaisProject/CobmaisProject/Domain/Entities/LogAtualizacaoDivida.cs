namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa o log de atualização de dívidas no sistema.
    /// Esta classe armazena informações detalhadas sobre as atualizações realizadas nas dívidas dos contratos.
    /// </summary>
    public class LogAtualizacaoDivida
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public decimal ValorOriginal { get; set; }
        public int DiasAtraso { get; set; }
        public string TipoContrato { get; set; }
        public decimal ValorAtualizado { get; set; }
        public decimal PercentualDesconto { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
