using CobmaisProject.Domain.Entities;

namespace CobmaisProject.Domain.Interfaces
{
    /// <summary>
    /// Interface responsável pelos repositórios de log de atualização de dívida.
    /// Define os métodos necessários para registrar logs de atualização de dívidas no sistema.
    /// </summary>
    public interface ILogAtualizacaoDividaRepository
    {
        Task RegistrarLogAsync(LogAtualizacaoDivida log);
    }
}
