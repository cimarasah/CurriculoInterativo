using AutoMapper;
using CurriculoInterativo.Api.DTOs.CurriculumDto;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Repositories.CertificationRepository;
using CurriculoInterativo.Api.Repositories.ContactRepository;
using CurriculoInterativo.Api.Repositories.ExperienceRepository;
using CurriculoInterativo.Api.Repositories.ProjectRepository;
using CurriculoInterativo.Api.Repositories.SkillRepository;
using CurriculoInterativo.Api.Repositories.SuggestionRepository;
using CurriculoInterativo.Api.Services.PdfService;

namespace CurriculoInterativo.Api.Services.CurriculumService
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPdfService _pdfService;
        private readonly IMapper _mapper;
        private readonly ILogger<CurriculumService> _logger;

        public CurriculumService(
            IContactRepository contactRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository,
            ICertificationRepository certificationRepository,
            IProjectRepository projectRepository,
            IPdfService pdfService,
            IMapper mapper,
            ILogger<CurriculumService> logger)
        {
            _contactRepository = contactRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
            _certificationRepository = certificationRepository;
            _projectRepository = projectRepository;
            _pdfService = pdfService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<byte[]> GenerateAndDownloadCurriculumAsync(
            string? ipAddress,
            string? userAgent)
        {
            try
            {


                // 2. Buscar e construir dados do currículo
                var curriculumData = await BuildCurriculumDataAsync();

                // 3. Gerar PDF
                var pdfBytes = _pdfService.GenerateCurriculumPdf(curriculumData);


                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao gerar currículo");
                throw;
            }
        }

        public async Task<CurriculumModel> BuildCurriculumDataAsync()
        {
            try
            {
                _logger.LogInformation("Construindo dados do currículo");

                // Buscar dados de todas as tabelas
                var contacts = await _contactRepository.GetAllAsync();
                var experiences = await _experienceRepository.GetAllAsync();
                var skills = await _skillRepository.GetAllAsync();
                var certifications = await _certificationRepository.GetAllAsync();
                var projects = await _projectRepository.GetAllAsync();

                var curriculumData = new CurriculumModel
                {
                    Contact = _mapper.Map<ContactModel>(contacts.FirstOrDefault()) ?? new ContactModel(),
                    Experiences = _mapper.Map<List<ExperienceModel>>(experiences),
                    Skills = _mapper.Map<List<SkillModel>>(skills),
                    Certifications = _mapper.Map<List<CertificationModel>>(certifications),
                    Projects = _mapper.Map<List<ProjectModel>>(projects),
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Dados do currículo construídos: {Experiences} experiências, {Skills} habilidades, {Certifications} certificações",
                    curriculumData.Experiences.Count,
                    curriculumData.Skills.Count,
                    curriculumData.Certifications.Count);

                return curriculumData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao construir dados do currículo");
                throw;
            }
        }

    }
}
