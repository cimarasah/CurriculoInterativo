using CurriculoInterativo.Api.DTOs.JobApplicationDto;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Repositories.CurriculumGenerationRepository;
using CurriculoInterativo.Api.Services.AIService;
using CurriculoInterativo.Api.Services.CurriculumService;
using CurriculoInterativo.Api.Utils.Exceptions;
using System.Text.Json;

namespace CurriculoInterativo.Api.Services.DedicatedCurriculumService
{
    public class DedicatedCurriculumService : IDedicatedCurriculumService
    {
        private readonly ICurriculumService _curriculumService;
        private readonly IAIService _aiService;
        private readonly ICurriculumGenerationRepository _generationRepository;
        private readonly ILogger<DedicatedCurriculumService> _logger;

        private const int MIN_JOB_DESCRIPTION_LENGTH = 50;

        public DedicatedCurriculumService(
            ICurriculumService curriculumService,
            IAIService aiService,
            ICurriculumGenerationRepository generationRepository,
            ILogger<DedicatedCurriculumService> logger)
        {
            _curriculumService = curriculumService;
            _aiService = aiService;
            _generationRepository = generationRepository;
            _logger = logger;
        }

        public async Task<DedicatedCurriculumMarkdownModel> GenerateDedicatedCurriculumAsync(
            JobApplicationRequest request,
            int? userId,
            string? ipAddress,
            string? userAgent)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando geração de currículo dedicado - Empresa: {Company}, UserId: {UserId}, IP: {Ip}",
                    request.CompanyName, userId, ipAddress);

                // 1. Validação: descrição da vaga deve ter no mínimo 50 caracteres
                if (string.IsNullOrWhiteSpace(request.JobDescription) || 
                    request.JobDescription.Length < MIN_JOB_DESCRIPTION_LENGTH)
                {
                    throw new ArgumentException(
                        $"A descrição da vaga deve ter no mínimo {MIN_JOB_DESCRIPTION_LENGTH} caracteres.");
                }

                // 2. Verificar rate limiting (antes da chamada à IA para proteger custos)
                var hasReachedLimit = await _generationRepository.HasReachedDailyLimitAsync(userId, ipAddress);
                if (hasReachedLimit)
                {
                    _logger.LogWarning(
                        "Limite diário de gerações atingido - UserId: {UserId}, IP: {Ip}",
                        userId, ipAddress);
                    throw new RateLimitExceededException(
                        "Limite de 10 gerações por dia atingido. Tente novamente amanhã.");
                }

                // 3. Buscar todos os dados do currículo
                var curriculumData = await _curriculumService.BuildCurriculumDataAsync();

                // 4. Serializar dados em JSON compacto
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                var curriculumDataJson = JsonSerializer.Serialize(curriculumData, jsonOptions);

                // 5. Chamar IA para gerar Markdown completo (única chamada)
                var markdown = await _aiService.GenerateDedicatedCurriculumMarkdownAsync(
                    request.JobDescription,
                    curriculumDataJson);

                // 6. Registrar geração para estatísticas
                var generation = new Entities.CurriculumGeneration
                {
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CompanyName = request.CompanyName,
                    JobDescriptionPreview = request.JobDescription.Length > 500
                        ? request.JobDescription.Substring(0, 500)
                        : request.JobDescription,
                    Status = "Success",
                    GeneratedAt = DateTime.UtcNow
                };

                await _generationRepository.RegisterGenerationAsync(generation);

                _logger.LogInformation("Currículo dedicado em Markdown gerado com sucesso");
                
                return new DedicatedCurriculumMarkdownModel
                {
                    Markdown = markdown,
                    CompanyName = request.CompanyName,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (RateLimitExceededException)
            {
                // Registrar tentativa bloqueada por rate limit
                var generation = new Entities.CurriculumGeneration
                {
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CompanyName = request.CompanyName,
                    JobDescriptionPreview = request.JobDescription?.Length > 500
                        ? request.JobDescription.Substring(0, 500)
                        : request.JobDescription,
                    Status = "RateLimited",
                    GeneratedAt = DateTime.UtcNow
                };
                await _generationRepository.RegisterGenerationAsync(generation);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar currículo dedicado");
                
                // Registrar falha
                var generation = new Entities.CurriculumGeneration
                {
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CompanyName = request.CompanyName,
                    JobDescriptionPreview = request.JobDescription?.Length > 500
                        ? request.JobDescription.Substring(0, 500)
                        : request.JobDescription,
                    Status = "Failed",
                    GeneratedAt = DateTime.UtcNow
                };
                await _generationRepository.RegisterGenerationAsync(generation);
                
                throw;
            }
        }
    }
}

