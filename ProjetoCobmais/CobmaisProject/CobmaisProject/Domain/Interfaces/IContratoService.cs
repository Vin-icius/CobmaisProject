using CobmaisProject.Application.DTOs;
using CobmaisProject.Domain.Entities;

namespace CobmaisProject.Domain.Interfaces
{
    /// <summary>
    /// Interface responsável pelos serviços relacionados ao gerenciamento de contratos.
    /// Esta interface define os métodos necessários para a importação, consulta e manipulação de contratos.
    /// </summary>
    public interface IContratoService
    {
        Task ImportarContratosAsync(List<ContratoCsvDto> contratos);

        Task<Contrato> ObterContratoPorIdAsync(int id);

        Task<(List<Divida> DividasAtualizadas, List<string> TiposContratoComFalha)> AtualizarDividasAsync();
        void Exportar(List<Divida> dividas);

        Task<List<Contrato>> ObterTodosContratosAsync();
    }
}
