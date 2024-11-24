using CobmaisProject.Domain.Entities;
using CobmaisProject.Domain.Interfaces;
using System.Globalization;
using CobmaisProject.Application.DTOs;
using CobmaisProject.Infrastructure.ExternalServices;
using System.Text;

namespace CobmaisProject.Application.Service
{
    /// <summary>
    /// Serviço responsável pela lógica de negócios relacionada a contratos, incluindo importação, atualização e exportação.
    /// </summary>
    public class ContratoService : IContratoService
    {
        private readonly IContratoRepository _contratoRepository;
        private readonly CobmaisApiService _cobmaisApiService;
        private ILogAtualizacaoDividaRepository _logAtualizacaoDividaRepository;


        /// <summary>
        /// Construtor para injeção de dependências da camada de serviço.
        /// </summary>
        /// <param name="contratoRepository">Repositório para operações com contratos</param>
        /// <param name="cobmaisApiService">Service para interação com a API Cobmais</param>
        /// <param name="logAtualizacaoDividaRepository">Repositório para registros de atualização de dívidas</param>
        public ContratoService(IContratoRepository contratoRepository, CobmaisApiService cobmaisApiService, ILogAtualizacaoDividaRepository logAtualizacaoDividaRepository)
        {
            _contratoRepository = contratoRepository;
            _cobmaisApiService = cobmaisApiService;
            this._logAtualizacaoDividaRepository = logAtualizacaoDividaRepository;
        }

        /// <summary>
        /// Importa contratos a partir de uma lista de objetos DTO (ContratoCsvDto).
        /// O método verifica se a pessoa existe no banco, cria um cliente e tipo de contrato se necessário,
        /// e em seguida persiste os dados do contrato.
        /// </summary>
        /// <param name="contratos">Lista de contratos a serem importados</param>
        /// <returns>Uma tarefa assíncrona representando a operação de importação</returns>
        public async Task ImportarContratosAsync(List<ContratoCsvDto> contratos)
        {
            foreach (var dto in contratos)
            {
                var pessoaId = await _contratoRepository.ObterPessoaIdPorCpfAsync(dto.Cpf);
                if (pessoaId == 0)
                {
                    pessoaId = await _contratoRepository.CriarPessoaAsync(dto.Cliente, dto.Cpf);
                    await _contratoRepository.CriarClienteAsync(pessoaId, dto.Contrato);
                }

                var tipoContratoId = await _contratoRepository.ObterTipoContratoIdAsync(dto.TipoContrato);
                if (tipoContratoId == 0)
                {
                    tipoContratoId = await _contratoRepository.CriarTipoContratoAsync(dto.TipoContrato);
                }

                var contrato = new Contrato
                {
                    ClienteId = pessoaId,
                    Vencimento = dto.Vencimento,
                    Valor = dto.Valor,
                    TipoContratoId = tipoContratoId
                };

                await _contratoRepository.CriarContratoAsync(contrato);
            }
        }

        /// <summary>
        /// Obtém um contrato específico a partir de seu ID.
        /// </summary>
        /// <param name="id">ID do contrato a ser recuperado</param>
        /// <returns>O contrato correspondente ao ID informado</returns>
        public async Task<Contrato> ObterContratoPorIdAsync(int id)
        {
            return await _contratoRepository.ObterContratoPorIdAsync(id);
        }


        /// <summary>
        /// Obtém todos os contratos registrados no sistema.
        /// </summary>
        /// <returns>Uma lista de todos os contratos</returns>
        public async Task<List<Contrato>> ObterTodosContratosAsync()
        {
            return await _contratoRepository.ObterContratosAsync();
        }


        /// <summary>
        /// Atualiza as dívidas dos contratos, obtendo informações sobre o atraso e aplicando possíveis descontos de acordo com a API Cobmais.
        /// Em seguida, registra as atualizações em um log e retorna os dados das dívidas atualizadas.
        /// </summary>
        /// <returns>Uma tupla contendo as dívidas atualizadas e os tipos de contrato que falharam ao atualizar</returns>
        public async Task<(List<Divida> DividasAtualizadas, List<string> TiposContratoComFalha)> AtualizarDividasAsync()
        {
            var contratos = await _contratoRepository.ObterContratosAsync();
            var dividasAtualizadas = new List<Divida>();
            var tiposContratoComFalha = new List<string>();

            foreach (var contrato in contratos)
            {
                try
                {
                    var cliente = await _contratoRepository.ObterCpfPorClienteIdAsync(contrato.ClienteId);
                    if (cliente == null)
                    {
                        throw new Exception($"Cliente com ID {contrato.ClienteId} não encontrado.");
                    }

                    var diasAtraso = (DateTime.Now - contrato.Vencimento).Days;
                    var tipoContrato = await _contratoRepository.ObterDescricaoTipoContratoPorIdAsync(contrato.TipoContratoId);
                    if (tipoContrato == null)
                    {
                        throw new Exception($"Tipo de Contrato com ID {contrato.TipoContratoId} não encontrado.");
                    }

                    try
                    {
                        var apiResponse = await _cobmaisApiService.AtualizarDividaAsync(
                            (double)contrato.Valor, diasAtraso, tipoContrato);

                        var log = new LogAtualizacaoDivida
                        {
                            ContratoId = contrato.Id,
                            ValorOriginal = contrato.Valor,
                            DiasAtraso = diasAtraso,
                            TipoContrato = tipoContrato,
                            ValorAtualizado = (decimal)apiResponse.ValorAtualizado,
                            PercentualDesconto = (decimal)apiResponse.DescontoMaximo,
                            DataAtualizacao = DateTime.Now
                        };

                        await _logAtualizacaoDividaRepository.RegistrarLogAsync(log);

                        dividasAtualizadas.Add(new Divida
                        {
                            Cpf = cliente,
                            DataAtualizacao = DateTime.Now,
                            Contrato = contrato.Id,
                            ValorOriginal = contrato.Valor,
                            ValorAtualizado = (decimal)apiResponse.ValorAtualizado,
                            PercentualDesconto = (decimal)apiResponse.DescontoMaximo
                        });
                    }
                    catch
                    {
                        if (!tiposContratoComFalha.Contains(tipoContrato))
                        {
                            tiposContratoComFalha.Add(tipoContrato);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o contrato ID {contrato.Id}: {ex.Message}");
                }
            }

            return (dividasAtualizadas, tiposContratoComFalha);
        }


        /// <summary>
        /// Exporta as dívidas atualizadas para um arquivo CSV.
        /// O arquivo contém os detalhes das dívidas, incluindo CPF, data, contrato, valor original, valor atualizado e valor de desconto.
        /// </summary>
        /// <param name="dividas">Lista de dívidas a serem exportadas</param>
        public void Exportar(List<Divida> dividas)
        {
            var nomeArquivo = $"Divida-Atualizada-{DateTime.Now:dd-MM-yyyy}.csv";
            var diretorio = "C:\\ArquivosCSV\\Atualizados";
            var caminhoArquivo = Path.Combine(diretorio, nomeArquivo);

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            var linhas = new List<string>();
            linhas.Add("CPF;DATA;CONTRATO;VALOR ORIGINAL;VALOR ATUALIZADO;VALOR DESCONTO");

            foreach (var divida in dividas)
            {
                divida.ValorDesconto = divida.ValorAtualizado * (divida.PercentualDesconto / 100);

                var linha = string.Format(CultureInfo.InvariantCulture, "{0};{1:dd/MM/yyyy};{2};{3:N2};{4:N2};{5:N2}",
                    divida.Cpf,
                    divida.DataAtualizacao,
                    divida.Contrato,
                    divida.ValorOriginal,
                    divida.ValorAtualizado,
                    divida.ValorDesconto);

                linhas.Add(linha);
            }

            File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
        }

    }
}
