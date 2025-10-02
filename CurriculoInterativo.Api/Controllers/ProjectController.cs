using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.ProjectService;

namespace ProjectInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Retorna todas os projetos
        /// </summary>
        /// <returns>Lista de projetos</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetProjects()
        {
            try
            {
                var Projects = await _projectService.GetProjectsAsync();
                return Ok(Projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Retorna projetos por habilidade
        /// </summary>
        /// <param name="skill">Habilidade dos projetos</param>
        /// <returns>Lista de projetos por habilidade especificada</returns>
        [HttpGet("skill/{skillDto}")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsBySkill(SkillDto skillDto)
        {
            try
            {
                var ProjectsFiltrados = await _projectService.GetProjectsBySkillAsync(skillDto);
                return Ok(ProjectsFiltrados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        /// <summary>
        /// Retorna todas os projetos com a empresa
        /// </summary>
        /// <returns>Lista de projetos</returns>
        [HttpGet("projects-with-company")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsWithCompanyAsync()
        {
            try
            {
                var Projects = await _projectService.GetProjectsWithCompanyAsync();
                return Ok(Projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

    }
}

