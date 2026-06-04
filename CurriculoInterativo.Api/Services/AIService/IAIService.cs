using CurriculoInterativo.Api.Models;

namespace CurriculoInterativo.Api.Services.AIService
{
    public interface IAIService
    {
        /// <summary>
        /// Responde uma pergunta baseado na descrição da vaga e nos dados do currículo
        /// </summary>
        /// <param name="question">Pergunta/campo a ser respondido</param>
        /// <param name="jobDescription">Descrição completa da vaga</param>
        /// <param name="curriculumData">Dados do currículo do banco</param>
        /// <returns>Resposta da IA baseada apenas nos dados fornecidos</returns>
        Task<string> AnswerQuestionAsync(string question, string jobDescription, CurriculumModel curriculumData);

        /// <summary>
        /// Gera um currículo completo em Markdown baseado na descrição da vaga e dados profissionais serializados
        /// </summary>
        /// <param name="jobDescription">Descrição completa da vaga</param>
        /// <param name="curriculumDataJson">Dados do currículo serializados em JSON</param>
        /// <returns>Markdown estruturado do currículo personalizado</returns>
        Task<string> GenerateDedicatedCurriculumMarkdownAsync(string jobDescription, string curriculumDataJson);
    }
}

