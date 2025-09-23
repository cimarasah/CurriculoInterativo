using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.SkillService;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        /// <summary>
        /// Retorna as informações de habilidade
        /// </summary>
        /// <returns>Informações de habilidade</returns>
        [HttpGet]
        public async Task<ActionResult<SkillDto>> GetSkill()
        {
            try
            {
                var skill = await _skillService.GetSkillsAsync();
                return Ok(skill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}

