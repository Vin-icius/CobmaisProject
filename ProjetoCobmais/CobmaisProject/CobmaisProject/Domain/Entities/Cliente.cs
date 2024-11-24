namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa um cliente no sistema, que é uma extensão da classe Pessoa.
    /// A classe Cliente adiciona o campo "Contrato" para associar a pessoa a um contrato específico.
    /// </summary>
    public class Cliente : Pessoa
    {
        public string Contrato { get; set; }
    }
}
