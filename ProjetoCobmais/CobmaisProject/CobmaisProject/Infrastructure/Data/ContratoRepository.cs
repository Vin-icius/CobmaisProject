using CobmaisProject.Domain.Entities;
using CobmaisProject.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace CobmaisProject.Infrastructure.Data
{
    /// <summary>
    /// Implementação do repositório para operações de CRUD relacionadas ao contrato.
    /// Realiza consultas, inserções e atualizações no banco de dados usando SQL.
    /// </summary>
    public class ContratoRepository : IContratoRepository
    {
        private readonly SqlConnection _connection;

        public ContratoRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task CriarContratoAsync(Contrato contrato)
        {
            try
            {
                var query = @"
                    INSERT INTO Contrato (ClienteId, Vencimento, Valor, TipoContratoId)
                    VALUES (@ClienteId, @Vencimento, @Valor, @TipoContratoId)";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@ClienteId", contrato.ClienteId);
                command.Parameters.AddWithValue("@Vencimento", contrato.Vencimento);
                command.Parameters.AddWithValue("@Valor", contrato.Valor);
                command.Parameters.AddWithValue("@TipoContratoId", contrato.TipoContratoId);

                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar contrato: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> ObterPessoaIdPorCpfAsync(string cpf)
        {
            try
            {
                var query = "SELECT Id FROM Pessoa WHERE Cpf = @Cpf";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Cpf", cpf);

                await _connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter pessoa por CPF: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<long> ObterCpfPorClienteIdAsync(int clienteId)
        {
            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = @"
                    SELECT Pessoa.Cpf 
                    FROM Pessoa
                    INNER JOIN Cliente ON Pessoa.Id = Cliente.Id
                    WHERE Cliente.Id = @Id";
                command.Parameters.AddWithValue("@Id", clienteId);

                await _connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                if (result == null || !long.TryParse(result.ToString(), out var cpf))
                {
                    throw new Exception($"CPF não encontrado ou inválido para o ClienteId {clienteId}.");
                }

                return cpf;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter CPF por ClienteId: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> CriarPessoaAsync(string nome, string cpf)
        {
            try
            {
                var query = @"
                    INSERT INTO Pessoa (Nome, Cpf)
                    OUTPUT INSERTED.Id
                    VALUES (@Nome, @Cpf)";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Nome", nome);
                command.Parameters.AddWithValue("@Cpf", cpf);

                await _connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar pessoa: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task CriarClienteAsync(int pessoaId, string contrato)
        {
            try
            {
                var query = @"
                    INSERT INTO Cliente (Id, Contrato)
                    VALUES (@PessoaId, @Contrato)";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@PessoaId", pessoaId);
                command.Parameters.AddWithValue("@Contrato", contrato);

                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar cliente: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> ObterTipoContratoIdAsync(string tipoContrato)
        {
            try
            {
                var query = "SELECT Id FROM TipoContrato WHERE TipoContrato = @TipoContrato";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@TipoContrato", tipoContrato);

                await _connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter TipoContratoId: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> CriarTipoContratoAsync(string tipoContrato)
        {
            try
            {
                var query = @"
                    INSERT INTO TipoContrato (TipoContrato)
                    OUTPUT INSERTED.Id
                    VALUES (@TipoContrato)";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@TipoContrato", tipoContrato);

                await _connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar TipoContrato: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<string?> ObterDescricaoTipoContratoPorIdAsync(int tipoContratoId)
        {
            const string query = "SELECT TipoContrato FROM TipoContrato WHERE Id = @TipoContratoId";

            try
            {
                await using var command = _connection.CreateCommand();
                command.CommandText = query;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@TipoContratoId";
                parameter.Value = tipoContratoId;
                command.Parameters.Add(parameter);

                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }

                var result = await command.ExecuteScalarAsync();
                return result != DBNull.Value ? result as string : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter descrição do tipo de contrato: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }


        public async Task<Contrato> ObterContratoPorIdAsync(int id)
        {
            try
            {
                var query = @"
                    SELECT Id, ClienteId, Vencimento, Valor, TipoContratoId
                    FROM Contrato
                    WHERE Id = @Id";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Id", id);

                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                Contrato contrato = null;
                if (await reader.ReadAsync())
                {
                    contrato = new Contrato
                    {
                        Id = reader.GetInt32(0),
                        ClienteId = reader.GetInt32(1),
                        Vencimento = reader.GetDateTime(2),
                        Valor = reader.GetDecimal(3),
                        TipoContratoId = reader.GetInt32(4)
                    };
                }

                return contrato;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter contrato por Id: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<List<Contrato>> ObterContratosAsync()
        {
            try
            {
                var query = @"
                    SELECT Id, ClienteId, Vencimento, Valor, TipoContratoId
                    FROM Contrato";
                using var command = new SqlCommand(query, _connection);

                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                var contratos = new List<Contrato>();
                while (await reader.ReadAsync())
                {
                    contratos.Add(new Contrato
                    {
                        Id = reader.GetInt32(0),
                        ClienteId = reader.GetInt32(1),
                        Vencimento = reader.GetDateTime(2),
                        Valor = reader.GetDecimal(3),
                        TipoContratoId = reader.GetInt32(4)
                    });
                }

                return contratos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter contratos: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
