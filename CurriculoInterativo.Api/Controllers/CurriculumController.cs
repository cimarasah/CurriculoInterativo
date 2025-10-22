using CurriculoInterativo.Api.DTOs.CurriculumDto;
using CurriculoInterativo.Api.Services.CurriculumService;
using CurriculoInterativo.Api.Utils.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;
        private readonly ILogger<CurriculumController> _logger;

        public CurriculumController(
            ICurriculumService curriculumService,
            ILogger<CurriculumController> logger)
        {
            _curriculumService = curriculumService;
            _logger = logger;
        }

        /// <summary>
        /// Gera e faz download do currículo em PDF
        /// </summary>
        /// <param name="request">Dados do lead interessado</param>
        /// <returns>Arquivo PDF do currículo</returns>
        [HttpGet("download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadCurriculum()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Capturar IP e User-Agent para análise
                var ipAddress = IpAddressHelper.GetClientIpAddress(HttpContext);
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                _logger.LogInformation(
                    "Solicitação de download recebida - IP: {IP}",
                    ipAddress);

                // Gerar PDF
                var pdfBytes = await _curriculumService.GenerateAndDownloadCurriculumAsync(
                    ipAddress,
                    userAgent);

                // Retornar arquivo PDF
                var fileName = $"Curriculo_CimaraSa_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(
                    pdfBytes,
                    "application/pdf",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar download de currículo");
                return StatusCode(500, new
                {
                    message = "Erro ao gerar currículo. Por favor, tente novamente."
                });
            }
        }

        /// <summary>
        /// Retorna estatísticas de downloads (apenas para Owner)
        /// </summary>
        /// <returns>Estatísticas de downloads</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetDownloadStats()
        {
            try
            {
                // Implementação futura para dashboard de métricas
                return Ok(new
                {
                    message = "Estatísticas disponíveis em breve"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas");
                return StatusCode(500, new { message = "Erro ao buscar estatísticas" });
            }
        }
    }
}
