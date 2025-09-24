using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.Services;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.CertificationService;
using CurriculoInterativo.Api.Enums;
using CurriculoInterativo.Api.Utils.Exceptions;
using CurriculoInterativo.Api.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using CurriculoInterativo.Api.Controllers;

namespace CertificationInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;
        private readonly ILogger<CertificationController> _logger;


        public CertificationController(ICertificationService certificationService, ILogger<CertificationController> logger)
        {
            _certificationService = certificationService;
            _logger = logger;
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
        /// <summary>
        /// Retorna um certificação específico por ID
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CertificationDto>> GetCertification(int id)
        {
            var certification = await _certificationService.GetCertificationByIdAsync(id);
            if (certification == null)
                throw new NotFoundException($"Certificação com ID {id} não encontrado");

            return Ok(certification);
        }
        /// <summary>
        /// Cria um novo certificação
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<CertificationDto>> CreateCertification([FromBody] CertificationDto certificationDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Dados inválidos enviados");

            var userId = User.GetUserId();

            var createdCertification = await _certificationService.CreateCertificationAsync(certificationDto, userId);

            _logger.LogInformation("Certificação criado com sucesso pelo usuário: {UserId}", userId);

            return CreatedAtAction(nameof(GetCertification), new { id = createdCertification.Id }, createdCertification);
        }

        /// <summary>
        /// Atualiza um certificação existente
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<CertificationDto>> UpdateCertification(int id, [FromBody] CertificationDto certificationDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Dados inválidos enviados");

            var userId = User.GetUserId();

            var updatedCertification = await _certificationService.UpdateCertificationAsync(id, certificationDto, userId);

            if (updatedCertification == null)
                throw new NotFoundException($"Certificação com ID {id} não encontrado");

            _logger.LogInformation("Certificação {Id} atualizado com sucesso pelo usuário: {UserId}", id, userId);

            return Ok(updatedCertification);
        }

        /// <summary>
        /// Exclui um certificação
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var userId = User.GetUserId();

            await _certificationService.DeleteCertificationAsync(id, userId);

            _logger.LogInformation("Certificação {Id} excluído com sucesso pelo usuário: {UserId}", id, userId);

            return Ok(new { message = "Certificação excluído com sucesso" });
        }
    }
}

