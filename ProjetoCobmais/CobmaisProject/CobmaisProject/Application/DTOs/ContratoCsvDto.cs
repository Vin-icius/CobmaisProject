using CsvHelper.Configuration.Attributes;

namespace CobmaisProject.Application.DTOs
{
    /// <summary>
    /// Representa um contrato enviado via CSV para importação.
    /// Este objeto contém as informações necessárias para registrar um contrato na aplicação.
    /// </summary>
    public class ContratoCsvDto
    {

        [Name("CPF")]
        public string Cpf { get; set; }

        [Name("CLIENTE")]
        public string Cliente { get; set; }

        [Name("CONTRATO")]
        public string Contrato { get; set; }

        [Name("VENCIMENTO")]
        public DateTime Vencimento { get; set; }

        [Name("VALOR")]
        public decimal Valor { get; set; }

        [Name("TIPO DE CONTRATO")]
        public string TipoContrato { get; set; }
    }
}