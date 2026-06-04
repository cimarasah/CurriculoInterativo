using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.DTOs.CurriculumDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.SuggestionRepository;
using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Services.SuggestionService
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _repository;
        private readonly ILogger<SuggestionService> _logger;
        private readonly IMapper _mapper;



        public SuggestionService(
            ISuggestionRepository repository,
            ILogger<SuggestionService> logger, 
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task RegisterSuggestionAsync(
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

                await _repository.AddAsync(newSuggestion);

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
        public async Task<SuggestionRequest> CreateSuggestionAsync(SuggestionRequest suggestionDto)
        {
            if (suggestionDto == null)
                throw new ValidationException("Dados do contato não podem ser nulos");

            var contact = _mapper.Map<Suggestion>(suggestionDto);

            await _repository.AddAsync(contact);

            return _mapper.Map<SuggestionRequest>(contact);
        }
    }
}
