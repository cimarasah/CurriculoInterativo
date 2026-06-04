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

        // Endpoint removido - funcionalidade não implementada
    }
}

