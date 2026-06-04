using CurriculoInterativo.Api.DTOs.CurriculumDto;

namespace CurriculoInterativo.Api.Services.SuggestionService
{
    public interface ISuggestionService
    {
        Task RegisterSuggestionAsync(
            CurriculumDownloadRequest request,
            string? ipAddress,
            string? userAgent);
    }
}
