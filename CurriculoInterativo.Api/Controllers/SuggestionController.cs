using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.Models;
using CurriculoInterativo.Api.Services.SuggestionService;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuggestionController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionController(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        /// <summary>
        /// Retorna as informações de habilidade
        /// </summary>
        /// <returns>Informações de habilidade</returns>
        [HttpGet]
        public async Task<ActionResult<SuggestionResponse>> GetSuggestion()
        {
            try
            {
                var Suggestion = await _suggestionService.re();
                return Ok(Suggestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}

