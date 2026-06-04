using CurriculoInterativo.Api.DTOs.JobApplicationDto;
using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.DedicatedCurriculumService
{
    public interface IDedicatedCurriculumService
    {
        /// <summary>
        /// Gera um currículo dedicado em Markdown baseado na descrição da vaga
        /// </summary>
        /// <param name="request">Dados da vaga (descrição e nome da empresa)</param>
        /// <param name="userId">ID do usuário autenticado (null se não autenticado)</param>
        /// <param name="ipAddress">Endereço IP do cliente</param>
        /// <param name="userAgent">User-Agent do cliente</param>
        /// <returns>Modelo com Markdown do currículo gerado</returns>
        Task<DedicatedCurriculumMarkdownModel> GenerateDedicatedCurriculumAsync(
            JobApplicationRequest request,
            int? userId,
            string? ipAddress,
            string? userAgent);
    }
}

