using CobmaisProject.Domain.Entities;

namespace CobmaisProject.Domain.Interfaces
{
    /// <summary>
    /// Interface responsável pelas operações de persistência relacionadas aos contratos.
    /// Esta interface define os métodos necessários para acessar e manipular os dados dos contratos e suas entidades associadas no banco de dados.
    /// </summary>
    public interface IContratoRepository
    {
        Task CriarContratoAsync(Contrato contrato);
        Task<int> ObterPessoaIdPorCpfAsync(string cpf);
        Task<Int64> ObterCpfPorClienteIdAsync(int clienteId);
        Task<int> CriarPessoaAsync(string nome, string cpf);
        Task CriarClienteAsync(int pessoaId, string contrato);
        Task<int> ObterTipoContratoIdAsync(string tipoContrato);
        Task<string?> ObterDescricaoTipoContratoPorIdAsync(int tipoContratoId);
        Task<int> CriarTipoContratoAsync(string tipoContrato);
        Task<Contrato> ObterContratoPorIdAsync(int id);
        Task<List<Contrato>> ObterContratosAsync();

    }
}
