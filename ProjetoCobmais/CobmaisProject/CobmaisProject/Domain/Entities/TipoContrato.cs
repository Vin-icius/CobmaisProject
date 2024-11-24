namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa o tipo de contrato associado a um contrato.
    /// Esta classe é utilizada para identificar diferentes tipos de contratos disponíveis no sistema.
    /// </summary>
    public class TipoContrato
    {
        public int Id { get; set; }
        public string TipoContratoNome { get; set; }
    }
}
