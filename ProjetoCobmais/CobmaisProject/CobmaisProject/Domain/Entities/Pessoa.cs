namespace CobmaisProject.Domain.Entities
{
    /// <summary>
    /// Representa uma pessoa no sistema.
    /// A classe é utilizada para associar pessoas a contratos e clientes.
    /// </summary>
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
    }
}
