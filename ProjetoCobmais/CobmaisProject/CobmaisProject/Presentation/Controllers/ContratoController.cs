using CobmaisProject.Application.DTOs;
using CobmaisProject.Application.Service;
using CobmaisProject.Presentation.ViewModels;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CobmaisProject.Presentation.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de contratos e operações associadas a dívidas,incluindo
    /// a importação, atualização e exportação de informações de contratos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContratoController : ControllerBase
    {
        private readonly ContratoService _contratoService;

        public ContratoController(ContratoService service)
        {
            _contratoService = service;
        }

        /// <summary>
        /// Endpoint para a importação manual de arquivos CSV contendo informações sobre contratos.
        /// O arquivo deve ser enviado via formulário.
        /// </summary>
        /// <param name="model">O modelo que contém o arquivo CSV a ser processado.</param>
        /// <returns>Retorna uma resposta HTTP indicando o sucesso ou falha da operação.</returns>
        [HttpPost("importar")]
        public async Task<IActionResult> ImportarContrato([FromForm] CsvFileViewModel model)
        {
            if (model.Arquivo == null || model.Arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            try
            {
                using (var reader = new StreamReader(model.Arquivo.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";", // Define o delimitador como ponto e vírgula
                    HeaderValidated = null, // Ignora validação de cabeçalhos ausentes
                    MissingFieldFound = null // Ignora campos ausentes
                }))
                {

                    // Lê os registros do CSV
                    var contratos = csv.GetRecords<ContratoCsvDto>().ToList();

                    // Envia para a camada de serviço
                    await _contratoService.ImportarContratosAsync(contratos);

                    return Ok("Importação concluída com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao processar o arquivo: {ex.Message}");
            }
        }

        /// <summary>
        /// Endpoint para a importação automática de arquivos CSV localizados em um diretório específico.
        /// O diretório deve conter arquivos CSV no formato esperado.
        /// </summary>
        /// <returns>Retorna uma resposta HTTP indicando o sucesso ou falha da operação de importação.</returns>
        [HttpPost("importar-diretorio")]
        public async Task<IActionResult> ImportarContratosDeDiretorio()
        {
            try
            {
                // Caminho da pasta onde os arquivos CSV estão armazenados
                var diretorio = "C:\\ArquivosCSV\\";

                // Verifica se o diretório existe
                if (!Directory.Exists(diretorio))
                {
                    return NotFound($"O diretório {diretorio} não foi encontrado.");
                }

                // Busca os arquivos CSV na pasta
                var arquivosCsv = Directory.GetFiles(diretorio, "*.csv");
                if (arquivosCsv.Length == 0)
                {
                    return BadRequest("Nenhum arquivo CSV encontrado no diretório.");
                }

                foreach (var caminhoArquivo in arquivosCsv)
                {
                    using (var reader = new StreamReader(caminhoArquivo))
                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";", // Define o delimitador como ponto e vírgula
                        HeaderValidated = null, // Ignora validação de cabeçalhos ausentes
                        MissingFieldFound = null // Ignora campos ausentes
                    }))
                    {
                        // Lê os registros do CSV
                        var contratos = csv.GetRecords<ContratoCsvDto>().ToList();

                        // Envia para a camada de serviço
                        await _contratoService.ImportarContratosAsync(contratos);
                    }
                }

                return Ok("Importação concluída com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao processar os arquivos: {ex.Message}");
            }
        }

        /// <summary>
        /// Endpoint para listar todos os contratos cadastrados no sistema.
        /// Retorna uma lista com os contratos e seus dados associados.
        /// </summary>
        /// <returns>Uma resposta HTTP contendo a lista de contratos.</returns>
        [HttpGet("listar-todos-contratos")]
        public async Task<IActionResult> ListarContratos()
        {
            try
            {
                var contratos = await _contratoService.ObterTodosContratosAsync();
                return Ok(contratos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para atualizar e exportar as dívidas associadas aos contratos.
        /// A atualização dos valores de dívidas é feita com base nas informações fornecidas pela API externa.
        /// Após a atualização, as dívidas são exportadas para um arquivo CSV.
        /// </summary>
        /// <returns>Uma resposta HTTP confirmando a atualização e exportação das dívidas.</returns>
        [HttpPost("atualizar-dividas")]
        public async Task<IActionResult> AtualizarDividas()
        {
            try
            {
                var (dividasAtualizadas, tiposContratoComFalha) = await _contratoService.AtualizarDividasAsync();

                _contratoService.Exportar(dividasAtualizadas);

                return Ok(new
                {
                    Mensagem = "Dívidas atualizadas e exportadas com sucesso!",
                    TiposContratoComFalha = tiposContratoComFalha
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar dívidas: {ex.Message}");
            }
        }

    }
}
