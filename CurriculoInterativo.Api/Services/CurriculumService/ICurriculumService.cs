using CurriculoInterativo.Api.DTOs.CurriculumDto;
using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.CurriculumService
{
    public interface ICurriculumService
    {
        /// <summary>
        /// Gera o PDF do currículo e registra o lead
        /// </summary>
        Task<byte[]> GenerateAndDownloadCurriculumAsync(
            string? ipAddress,
            string? userAgent);

        /// <summary>
        /// Constrói o DTO com todos os dados do currículo
        /// </summary>
        Task<CurriculumModel> BuildCurriculumDataAsync();
    }
}
