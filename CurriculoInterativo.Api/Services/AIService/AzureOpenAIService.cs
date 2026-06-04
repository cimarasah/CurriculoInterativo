using Azure;
using Azure.AI.OpenAI;
using CurriculoInterativo.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace CurriculoInterativo.Api.Services.AIService
{
    public class AzureOpenAIService : IAIService
    {
        private readonly Azure.AI.OpenAI.OpenAIClient _client;
        private readonly string _deploymentName;
        private readonly ILogger<AzureOpenAIService> _logger;

        public AzureOpenAIService(IConfiguration configuration, ILogger<AzureOpenAIService> logger)
        {
            var endpoint = configuration["AzureOpenAISettings:Endpoint"] 
                ?? throw new InvalidOperationException("AzureOpenAI Endpoint não configurado");
            var apiKey = configuration["AzureOpenAISettings:ApiKey"] 
                ?? throw new InvalidOperationException("AzureOpenAI ApiKey não configurada");
            _deploymentName = configuration["AzureOpenAISettings:DeploymentName"] 
                ?? throw new InvalidOperationException("AzureOpenAI DeploymentName não configurado");

            _client = new Azure.AI.OpenAI.OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            _logger = logger;
        }

        public async Task<string> AnswerQuestionAsync(string question, string jobDescription, CurriculumModel curriculumData)
        {
            try
            {
                var prompt = BuildPrompt(question, jobDescription, curriculumData);

                var chatCompletionsOptions = new Azure.AI.OpenAI.ChatCompletionsOptions
                {
                    DeploymentName = _deploymentName,
                    Messages =
                    {
                        new Azure.AI.OpenAI.ChatRequestSystemMessage("Você é um assistente especializado em análise de currículos e vagas de emprego. Sua função é analisar a descrição da vaga e os dados do currículo fornecidos, e responder perguntas de forma precisa e objetiva, usando APENAS as informações disponíveis nos dados fornecidos. Se uma informação não estiver disponível nos dados, responda 'Não tenho certeza' ou 'Essa informação não está disponível no currículo'."),
                        new Azure.AI.OpenAI.ChatRequestUserMessage(prompt)
                    },
                    Temperature = 0.3f,
                    MaxTokens = 500
                };

                var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
                var answer = response.Value.Choices[0].Message.Content;

                _logger.LogInformation("Resposta da IA gerada para pergunta: {Question}", question);
                return answer ?? "Não foi possível gerar uma resposta.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao chamar Azure OpenAI para pergunta: {Question}", question);
                return "Erro ao processar a pergunta. Tente novamente.";
            }
        }

        private string BuildPrompt(string question, string jobDescription, CurriculumModel curriculumData)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("## DESCRIÇÃO DA VAGA:");
            sb.AppendLine(jobDescription);
            sb.AppendLine();
            
            sb.AppendLine("## DADOS DO CURRÍCULO:");
            sb.AppendLine();
            
            // Contato
            if (curriculumData.Contact != null)
            {
                sb.AppendLine("### Informações de Contato:");
                sb.AppendLine($"Nome: {curriculumData.Contact.Name}");
                sb.AppendLine($"Email: {curriculumData.Contact.Email}");
                sb.AppendLine($"Telefone: {curriculumData.Contact.Phone}");
                sb.AppendLine($"Localização: {curriculumData.Contact.Location}");
                sb.AppendLine();
            }
            
            // Experiências
            if (curriculumData.Experiences.Any())
            {
                sb.AppendLine("### Experiências Profissionais:");
                foreach (var exp in curriculumData.Experiences)
                {
                    sb.AppendLine($"- Empresa: {exp.Company}");
                    sb.AppendLine($"  Período: {exp.StartDate:MM/yyyy} - {(exp.EndDate.HasValue ? exp.EndDate.Value.ToString("MM/yyyy") : "Atual")}");
                    sb.AppendLine($"  Localização: {exp.Location}");
                    sb.AppendLine($"  Descrição: {exp.Description}");
                    sb.AppendLine();
                }
            }
            
            // Skills
            if (curriculumData.Skills.Any())
            {
                sb.AppendLine("### Habilidades Técnicas:");
                foreach (var skill in curriculumData.Skills)
                {
                    sb.AppendLine($"- {skill.Name} ({skill.Category}) - Nível: {skill.ProficiencyLevel}");
                }
                sb.AppendLine();
            }
            
            // Projetos
            if (curriculumData.Projects.Any())
            {
                sb.AppendLine("### Projetos:");
                foreach (var project in curriculumData.Projects)
                {
                    sb.AppendLine($"- {project.Name}");
                    sb.AppendLine($"  Descrição: {project.Description}");
                    sb.AppendLine($"  Tecnologias: {string.Join(", ", project.Skills.Select(s => s.Name))}");
                    sb.AppendLine();
                }
            }
            
            // Certificações
            if (curriculumData.Certifications.Any())
            {
                sb.AppendLine("### Certificações:");
                foreach (var cert in curriculumData.Certifications)
                {
                    sb.AppendLine($"- {cert.Name} - {cert.Institution} ({cert.ObtainedDate:MM/yyyy})");
                }
                sb.AppendLine();
            }
            
            sb.AppendLine("## PERGUNTA:");
            sb.AppendLine(question);
            sb.AppendLine();
            sb.AppendLine("## INSTRUÇÕES:");
            sb.AppendLine("Analise a descrição da vaga e os dados do currículo fornecidos acima.");
            sb.AppendLine("Responda a pergunta de forma objetiva e específica, mostrando como o candidato atende aos requisitos da vaga.");
            sb.AppendLine("Use APENAS as informações disponíveis nos dados do currículo.");
            sb.AppendLine("Se a informação não estiver disponível, responda 'Não tenho certeza' ou 'Essa informação não está disponível no currículo'.");
            sb.AppendLine("Seja específico: mencione empresas, projetos, tecnologias e períodos quando relevante.");
            sb.AppendLine("Mantenha a resposta concisa (máximo 3-4 frases).");
            
            return sb.ToString();
        }

        public async Task<string> GenerateDedicatedCurriculumMarkdownAsync(string jobDescription, string curriculumDataJson)
        {
            // Este serviço não é usado atualmente, mas precisa implementar a interface
            throw new NotImplementedException("Este método não está implementado para AzureOpenAI. Use GoogleGeminiService.");
        }
    }
}

