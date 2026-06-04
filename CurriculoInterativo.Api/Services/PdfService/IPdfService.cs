using CurriculoInterativo.Api.DTOs.JobApplicationDto;
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

        /// <summary>
        /// Gera um PDF do currículo dedicado à vaga
        /// </summary>
        /// <param name="model">Dados do currículo dedicado gerado pela IA</param>
        /// <returns>Array de bytes do PDF gerado</returns>
        byte[] GenerateDedicatedCurriculumPdf(DedicatedCurriculumModel model);

        /// <summary>
        /// Gera um PDF a partir de Markdown
        /// </summary>
        /// <param name="markdown">Conteúdo em Markdown</param>
        /// <returns>Array de bytes do PDF gerado</returns>
        byte[] GeneratePdfFromMarkdown(string markdown);
    }
}
