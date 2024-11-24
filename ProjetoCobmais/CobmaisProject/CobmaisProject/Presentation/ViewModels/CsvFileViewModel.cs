using System.ComponentModel.DataAnnotations;

namespace CobmaisProject.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel usado para representar o arquivo CSV enviado pelo usuário.
    /// </summary>
    public class CsvFileViewModel
    {
        [Required]
        public IFormFile Arquivo { get; set; }
    }
}
