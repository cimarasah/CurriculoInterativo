using CurriculoInterativo.Api.DTOs.JobApplicationDto;
using CurriculoInterativo.Api.Services.DedicatedCurriculumService;
using CurriculoInterativo.Api.Services.PdfService;
using CurriculoInterativo.Api.Utils.Exceptions;
using CurriculoInterativo.Api.Utils.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DedicatedCurriculumController : ControllerBase
    {
        private readonly IDedicatedCurriculumService _dedicatedCurriculumService;
        private readonly IPdfService _pdfService;
        private readonly ILogger<DedicatedCurriculumController> _logger;

        public DedicatedCurriculumController(
            IDedicatedCurriculumService dedicatedCurriculumService,
            IPdfService pdfService,
            ILogger<DedicatedCurriculumController> logger)
        {
            _dedicatedCurriculumService = dedicatedCurriculumService;
            _pdfService = pdfService;
            _logger = logger;
        }

        /// <summary>
        /// Gera um currículo dedicado baseado na descrição da vaga (acesso público para recrutadores)
        /// </summary>
        /// <param name="request">Dados da vaga (descrição e nome da empresa)</param>
        /// <returns>Arquivo PDF do currículo dedicado</returns>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(429)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerateDedicatedCurriculum([FromBody] JobApplicationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Capturar IP e User-Agent
                var ipAddress = IpAddressHelper.GetClientIpAddress(HttpContext);
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Obter UserId se autenticado (null se recrutador não autenticado)
                int? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out var parsedUserId))
                    {
                        userId = parsedUserId;
                    }
                }

                _logger.LogInformation(
                    "Solicitação de currículo dedicado recebida - Empresa: {Company}, UserId: {UserId}, IP: {Ip}",
                    request.CompanyName ?? "Não informada", userId, ipAddress);

                // Gerar currículo dedicado em Markdown usando IA
                var dedicatedCurriculum = await _dedicatedCurriculumService.GenerateDedicatedCurriculumAsync(
                    request,
                    userId,
                    ipAddress,
                    userAgent);

                // Gerar PDF a partir do Markdown
                var pdfBytes = _pdfService.GeneratePdfFromMarkdown(dedicatedCurriculum.Markdown);

                // Retornar arquivo PDF
                var fileName = $"Curriculo_Dedicado_{request.CompanyName?.Replace(" ", "_") ?? "Vaga"}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                return File(
                    pdfBytes,
                    "application/pdf",
                    fileName);
            }
            catch (RateLimitExceededException ex)
            {
                _logger.LogWarning(ex, "Limite de gerações diárias atingido");
                return StatusCode(429, new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validação falhou");
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar geração de currículo dedicado");
                return StatusCode(500, new
                {
                    message = "Erro ao gerar currículo dedicado. Por favor, tente novamente."
                });
            }
        }
    }
}

