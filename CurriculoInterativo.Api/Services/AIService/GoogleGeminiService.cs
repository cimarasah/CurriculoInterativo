using CurriculoInterativo.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CurriculoInterativo.Api.Services.AIService
{
    public class GoogleGeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelName;
        private readonly ILogger<GoogleGeminiService> _logger;

        public GoogleGeminiService(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            ILogger<GoogleGeminiService> logger)
        {
            _apiKey = configuration["GoogleGeminiSettings:ApiKey"]
                ?? throw new InvalidOperationException("Google Gemini ApiKey não configurada");
            _modelName = configuration["GoogleGeminiSettings:ModelName"] ?? "gemini-1.5-flash-latest";
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
            _logger = logger;
        }

        public async Task<string> AnswerQuestionAsync(string question, string jobDescription, CurriculumModel curriculumData)
        {
            try
            {
                var prompt = BuildPrompt(question, jobDescription, curriculumData);

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = prompt
                                }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        maxOutputTokens = 500
                    }
                };

                var url = $"models/{_modelName}:generateContent";
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro na API do Google Gemini: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    return "Erro ao processar a pergunta. Tente novamente.";
                }

                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                var answer = result?.candidates?[0]?.content?.parts?[0]?.text;

                _logger.LogInformation("Resposta da IA (Gemini) gerada para pergunta: {Question}", question);
                return answer ?? "Não foi possível gerar uma resposta.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao chamar Google Gemini para pergunta: {Question}", question);
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
            sb.AppendLine("Você é um assistente especializado em análise de currículos e vagas de emprego.");
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
            try
            {
                var systemPrompt = @"Você é um especialista em Recrutamento e Seleção de Tecnologia. Sua tarefa é atuar como um motor de transformação de dados para gerar um currículo altamente otimizado para uma vaga específica.

DIRETRIZES DE SEGURANÇA E QUALIDADE:
- FONTE ÚNICA DA VERDADE: Utilize apenas as informações fornecidas no contexto do usuário. É terminantemente proibido inventar experiências, datas, empresas ou tecnologias.
- FOCO NA VAGA: Selecione as experiências e projetos que possuem maior correlação com os requisitos da vaga descrita.
- LINGUAGEM: Use verbos de ação e foque em resultados e conquistas técnicas.
- ESTRUTURA: O conteúdo deve ser organizado de forma lógica para leitura humana e sistemas de triagem (ATS).";

                var userPrompt = $@"🟢 DESCRIÇÃO DA VAGA (INPUT DO USUÁRIO)
{jobDescription}

🔵 MEUS DADOS PROFISSIONAIS (FONTE DO BANCO DE DADOS)
{curriculumDataJson}

📋 TAREFA
Com base na descrição da vaga e nos meus dados acima, gere o conteúdo para um currículo personalizado. O retorno deve conter as seguintes seções:

1. Informações Básicas, Links e Contatos: Nome, cargo atual, LinkedIn, GitHub e meios de contato.
2. Resumo Profissional: Um parágrafo de 3 a 4 linhas destacando meus anos de experiência e como minhas principais expertises resolvem os problemas descritos na vaga.
3. Experiências e Projetos: Selecione as experiências e projetos mais relevantes. Para cada um, descreva responsabilidades e tecnologias utilizadas, priorizando o que a vaga pede.
4. Habilidades e Certificados: Liste as Hard Skills e Certificações que coincidem com os requisitos da vaga.

FORMATO DE RESPOSTA: Retorne o conteúdo em formato Markdown estruturado, para que eu possa converter diretamente em PDF. Não adicione comentários introdutórios ou conclusivos, retorne apenas o texto do currículo.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt },
                                new { text = userPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        maxOutputTokens = 4000
                    }
                };

                var url = $"models/{_modelName}:generateContent";
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro na API do Google Gemini ao gerar currículo dedicado: {StatusCode} - {Error}",
                        response.StatusCode, errorContent);
                    throw new InvalidOperationException($"Erro ao gerar currículo dedicado: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                var markdown = result?.candidates?[0]?.content?.parts?[0]?.text;

                if (string.IsNullOrWhiteSpace(markdown))
                {
                    _logger.LogError("Resposta vazia do Google Gemini ao gerar currículo dedicado");
                    throw new InvalidOperationException("Não foi possível gerar o currículo. A resposta da IA estava vazia.");
                }

                _logger.LogInformation("Currículo dedicado em Markdown gerado com sucesso");
                return markdown;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao chamar Google Gemini para gerar currículo dedicado");
                throw;
            }
        }

        // Classes para deserializar a resposta do Gemini
        private class GeminiResponse
        {
            public List<Candidate>? candidates { get; set; }
        }

        private class Candidate
        {
            public Content? content { get; set; }
        }

        private class Content
        {
            public List<Part>? parts { get; set; }
        }

        private class Part
        {
            public string? text { get; set; }
        }
    }
}

