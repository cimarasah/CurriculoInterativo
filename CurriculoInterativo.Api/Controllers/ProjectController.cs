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
        [HttpGet("{idSkill}")]
        public async Task<ActionResult<ListProjectBySkillDto>> GetProjectsBySkill(int idSkill)
        {
            try
            {
                var ProjectsFiltrados = await _projectService.GetProjectsBySkillAsync(idSkill);
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

