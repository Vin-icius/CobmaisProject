using CobmaisProject.Domain.Entities;
using CobmaisProject.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace CobmaisProject.Infrastructure.Data
{
    /// <summary>
    /// Implementação do repositório para registrar logs de atualização de dívidas no banco de dados.
    /// </summary>
    public class LogAtualizacaoDividaRepository : ILogAtualizacaoDividaRepository
    {
        private readonly SqlConnection _connection;

        public LogAtualizacaoDividaRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task RegistrarLogAsync(LogAtualizacaoDivida log)
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO LogAtualizacaoDivida (ContratoId, ValorOriginal, DiasAtraso, TipoContrato, ValorAtualizado, PercentualDesconto)
                VALUES (@ContratoId, @ValorOriginal, @DiasAtraso, @TipoContrato, @ValorAtualizado, @PercentualDesconto)";

            command.Parameters.AddWithValue("@ContratoId", log.ContratoId);
            command.Parameters.AddWithValue("@ValorOriginal", log.ValorOriginal);
            command.Parameters.AddWithValue("@DiasAtraso", log.DiasAtraso);
            command.Parameters.AddWithValue("@TipoContrato", log.TipoContrato);
            command.Parameters.AddWithValue("@ValorAtualizado", log.ValorAtualizado);
            command.Parameters.AddWithValue("@PercentualDesconto", log.PercentualDesconto);

            await _connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }
    }
}
