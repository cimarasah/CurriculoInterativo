using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.Services.ExperienceService;
using CurriculoInterativo.Api.DTOs.ExperienceDto;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        /// <summary>
        /// Retorna o histórico de experiências profissionais
        /// </summary>
        /// <returns>Lista de experiências profissionais</returns>
        [HttpGet]
        public async Task<ActionResult<List<ExperienceResponse>>> GetExperiences()
        {
            try
            {
                var e = await _experienceService.GetExperiencesMiniAsync();
                return Ok(e);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}

