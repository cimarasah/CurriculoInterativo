using CurriculoInterativo.Api.DTOs.CurriculumDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.SuggestionRepository;

namespace CurriculoInterativo.Api.Services.SuggestionService
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _suggestionRepository;
        private readonly ILogger<SuggestionService> _logger;


        public SuggestionService(
            ISuggestionRepository suggestionRepository,
            ILogger<SuggestionService> logger)
        {
            _suggestionRepository = suggestionRepository;
            _logger = logger;
        }
        private async Task RegisterSuggestionAsync(
            CurriculumDownloadRequest request,
            string? ipAddress,
            string? userAgent)
        {
            try
            {
                var newSuggestion = new Suggestion
                {
                    Name = request.Name,
                    Email = request.Email,
                    Company = request.Company,
                    Position = request.Position,
                    Message = request.Message,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    RequestedAt = DateTime.UtcNow,
                };

                await _suggestionRepository.AddAsync(newSuggestion);

                _logger.LogInformation(
                    "Nova sugestão registrada: {Email} - {Name}",
                    request.Email,
                    request.Name);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao registrar sugestão: {Email}",
                    request.Email);
                throw;
            }
        }
    }
}
