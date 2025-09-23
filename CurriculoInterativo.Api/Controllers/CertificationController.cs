using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.Services;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.CertificationService;
using CurriculoInterativo.Api.Enums;

namespace CertificationInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        /// <summary>
        /// Retorna todas as certificações
        /// </summary>
        /// <returns>Lista de certificações</returns>
        [HttpGet]
        public async Task<ActionResult<List<CertificationDto>>> GetCertifications()
        {
            try
            {
                var Certifications = await _certificationService.GetCertificationsAsync();
                return Ok(Certifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Retorna certificações por categoria
        /// </summary>
        /// <param name="categoria">Categoria das certificações</param>
        /// <returns>Lista de certificações da categoria especificada</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<CertificationDto>>> GetCertificationsByCategory(SkillCategory category)
        {
            try
            {
                var CertificationsFiltradas = await _certificationService.GetCertificationsByCategoryAsync(category);
                return Ok(CertificationsFiltradas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}

