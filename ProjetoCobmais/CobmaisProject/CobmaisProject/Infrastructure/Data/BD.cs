using Microsoft.Data.SqlClient;

namespace CobmaisProject.Infrastructure.Data
{
    /// <summary>
    /// Classe responsável pela criação e gerenciamento de conexões com o banco de dados.
    /// Utiliza a string de conexão armazenada nas variáveis de ambiente para estabelecer uma conexão com o banco.
    /// </summary>
    public class BD
    {
        public SqlConnection CriarConexao()
        {
            string strCon = Environment.GetEnvironmentVariable("stringConexao");
            SqlConnection conexao = new SqlConnection(strCon);
            return conexao;
        }
    }
}
