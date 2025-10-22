using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.PdfService
{
    public interface IPdfService
    {
        /// <summary>
        /// Gera um PDF do currículo com os dados fornecidos
        /// </summary>
        /// <param name="data">Dados do currículo</param>
        /// <returns>Array de bytes do PDF gerado</returns>
        byte[] GenerateCurriculumPdf(CurriculumModel model);
    }
}
