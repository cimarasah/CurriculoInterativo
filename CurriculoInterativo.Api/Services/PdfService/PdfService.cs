using CurriculoInterativo.Api.DTOs.JobApplicationDto;
using CurriculoInterativo.Api.Models;
using Markdig;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace CurriculoInterativo.Api.Services.PdfService
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateCurriculumPdf(CurriculumModel data)
        {
            // Define licença Community do QuestPDF (gratuita para uso não comercial)
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    // Cabeçalho com informações de contato
                    page.Header().Element(c => ComposeHeader(c, data.Contact));

                    // Conteúdo principal
                    page.Content().Element(c => ComposeContent(c, data));

                    // Rodapé com data/hora de geração
                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Currículo gerado em: ");
                        text.Span($"{data.GeneratedAt:dd/MM/yyyy HH:mm}")
                            .FontSize(8)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, ContactModel contact)
        {
            container.Column(column =>
            {
                // Nome
                column.Item().Text(contact.Name)
                    .FontSize(26)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                // Informações de contato
                column.Item().PaddingTop(8).Row(row =>
                {
                    row.AutoItem().Text($"{contact.Email}").FontSize(10);
                    row.AutoItem().PaddingLeft(15).Text($"{contact.Phone}").FontSize(10);
                });

                column.Item().Row(row =>
                {
                    row.AutoItem().Text($"{contact.Location}").FontSize(10);
                    if (!string.IsNullOrEmpty(contact.LinkedIn))
                    {
                        row.AutoItem().PaddingLeft(15).Text($"{contact.LinkedIn}").FontSize(9).Italic();
                    }
                });

                if (!string.IsNullOrEmpty(contact.GitHub))
                {
                    column.Item().Text($"{contact.GitHub}").FontSize(9).Italic();
                }

                // Linha separadora
                column.Item().PaddingTop(12).LineHorizontal(2).LineColor(Colors.Blue.Darken2);
            });
        }

        private void ComposeContent(IContainer container, CurriculumModel data)
        {
            container.Column(column =>
            {
                // Experiências Profissionais
                if (data.Experiences.Any())
                {
                    column.Item().Element(c => ComposeExperiences(c, data.Experiences));
                }

                // Habilidades Técnicas
                if (data.Skills.Any())
                {
                    column.Item().PaddingTop(20).Element(c => ComposeSkills(c, data.Skills));
                }

                // Certificações
                if (data.Certifications.Any())
                {
                    column.Item().PaddingTop(20).Element(c => ComposeCertifications(c, data.Certifications));
                }

                // Projetos (opcional, se quiser incluir)
                if (data.Projects.Any())
                {
                    column.Item().PaddingTop(20).Element(c => ComposeProjects(c, data.Projects));
                }
            });
        }

        private void ComposeExperiences(IContainer container, List<ExperienceModel> experiences)
        {
            container.Column(column =>
            {
                // Título da seção
                column.Item().Text("EXPERIÊNCIA PROFISSIONAL")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingBottom(8).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                // Listar experiências ordenadas por data (mais recente primeiro)
                foreach (var exp in experiences.OrderByDescending(e => e.StartDate))
                {
                    column.Item().PaddingTop(12).Column(expColumn =>
                    {
                        // Nome da empresa
                        expColumn.Item().Text(exp.Company)
                            .FontSize(13)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        // Período
                        var startDate = exp.StartDate.ToString("MM/yyyy");
                        var endDate = exp.EndDate.HasValue
                            ? exp.EndDate.Value.ToString("MM/yyyy")
                            : "Atual";

                        expColumn.Item().PaddingTop(3).Text($"{startDate} - {endDate}")
                            .FontSize(10)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);

                        // Localização
                        if (!string.IsNullOrEmpty(exp.Location))
                        {
                            expColumn.Item().Text($"📍 {exp.Location}")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Medium);
                        }

                        // Descrição
                        if (!string.IsNullOrEmpty(exp.Description))
                        {
                            expColumn.Item().PaddingTop(5).Text(exp.Description)
                                .FontSize(10)
                                .LineHeight(1.4f)
                                .FontColor(Colors.Grey.Darken2);
                        }
                    });
                }
            });
        }

        private void ComposeSkills(IContainer container, List<SkillModel> skills)
        {
            container.Column(column =>
            {
                column.Item().Text("HABILIDADES TÉCNICAS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingBottom(8).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                // Agrupar skills por categoria
                var groupedSkills = skills
                    .GroupBy(s => s.Category)
                    .OrderBy(g => g.Key);

                foreach (var group in groupedSkills)
                {
                    column.Item().PaddingTop(10).Column(skillColumn =>
                    {
                        // Nome da categoria
                        skillColumn.Item().Text(GetCategoryDisplayName(group.Key))
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Grey.Darken2);

                        // Skills separadas por vírgula
                        var skillNames = string.Join(", ", group.Select(s => s.Name));
                        skillColumn.Item().PaddingTop(3).Text(skillNames)
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });
                }
            });
        }

        private void ComposeCertifications(IContainer container, List<CertificationModel> certifications)
        {
            container.Column(column =>
            {
                column.Item().Text("CERTIFICAÇÕES")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingBottom(8).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                foreach (var cert in certifications.OrderByDescending(c => c.ObtainedDate))
                {
                    column.Item().PaddingTop(8).Column(certColumn =>
                    {
                        certColumn.Item().Text(cert.Name)
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        certColumn.Item().PaddingTop(2).Text($"{cert.Institution} - {cert.ObtainedDate:MM/yyyy}")
                            .FontSize(10)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);

                        if (!string.IsNullOrEmpty(cert.CertificateUrl))
                        {
                            certColumn.Item().Text($"🔗 {cert.CertificateUrl}")
                                .FontSize(8)
                                .FontColor(Colors.Blue.Medium);
                        }
                    });
                }
            });
        }

        private void ComposeProjects(IContainer container, List<ProjectModel> projects)
        {
            container.Column(column =>
            {
                column.Item().Text("PROJETOS RELEVANTES")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingBottom(8).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                // Limitar a top 5 projetos mais recentes
                foreach (var project in projects.OrderByDescending(p => p.StartDate).Take(5))
                {
                    column.Item().PaddingTop(10).Column(projColumn =>
                    {
                        projColumn.Item().Text(project.Name)
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        if (!string.IsNullOrEmpty(project.Position))
                        {
                            projColumn.Item().Text(project.Position)
                                .FontSize(10)
                                .Italic()
                                .FontColor(Colors.Blue.Medium);
                        }

                        projColumn.Item().PaddingTop(3).Text(project.Description)
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);

                        // Tecnologias usadas
                        if (project.Skills.Any())
                        {
                            var techStack = string.Join(", ", project.Skills.Select(s => s.Name));
                            projColumn.Item().PaddingTop(3).Text($"Tecnologias: {techStack}")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Medium);
                        }
                    });
                }
            });
        }

        private string GetCategoryDisplayName(Enums.SkillCategory category)
        {
            return category switch
            {
                Enums.SkillCategory.Backend => "Backend",
                Enums.SkillCategory.Frontend => "Frontend",
                Enums.SkillCategory.Database => "Banco de Dados",
                Enums.SkillCategory.DevOps => "DevOps",
                Enums.SkillCategory.ProgrammingLanguage => "Linguagens de Programação",
                Enums.SkillCategory.Management => "Gestão e Metodologias",
                _ => category.ToString()
            };
        }

        public byte[] GenerateDedicatedCurriculumPdf(DedicatedCurriculumModel model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    // Cabeçalho
                    page.Header().Element(c => ComposeDedicatedHeader(c, model));

                    // Conteúdo principal
                    page.Content().Element(c => ComposeDedicatedContent(c, model));

                    // Rodapé
                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Currículo dedicado gerado em: ");
                        text.Span($"{model.GeneratedAt:dd/MM/yyyy HH:mm}")
                            .FontSize(8)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeDedicatedHeader(IContainer container, DedicatedCurriculumModel model)
        {
            container.Column(column =>
            {
                if (!string.IsNullOrEmpty(model.CompanyName))
                {
                    column.Item().Text($"Currículo Dedicado - {model.CompanyName}")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);
                    column.Item().PaddingTop(8);
                }

                column.Item().Text("INFORMAÇÕES DO CANDIDATO")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().PaddingTop(12).LineHorizontal(2).LineColor(Colors.Blue.Darken2);
            });
        }

        private void ComposeDedicatedContent(IContainer container, DedicatedCurriculumModel model)
        {
            container.Column(column =>
            {
                AddField(column, "Candidato", model.Candidato);
                AddField(column, "Residência", model.Residencia);
                AddField(column, "Graduação", model.Graduacao);
                AddField(column, "Atuação em empresas financeiras", model.AtuacaoEmpresasFinanceiras);
                AddField(column, "Tempo como DEV", model.TempoComoDev);
                AddField(column, "Tempo como .NET", model.TempoComoDotNet);
                AddField(column, "Tempo com Java", model.TempoComJava);
                AddField(column, "Principais tecnologias que já atuou", model.PrincipaisTecnologias);
                AddField(column, "Mensageria (RabbitMQ, Amazon SQS, EC2, Kafka)", model.Mensageria);
                AddField(column, "Aplicar sólidos skills de API design, Restful e Microservices", model.ApiDesignRestfulMicroservices);
                AddField(column, "Conhecimento em Cloud (preferencialmente AWS)", model.ConhecimentoCloud);
                AddField(column, "Docker e/ou Kubernetes", model.DockerKubernetes);
                AddField(column, "Esteiras CI/CD", model.EsteirasCICD);
                AddField(column, "Serverless", model.Serverless);
                AddField(column, "Conhecimento de aplicações que utilizam banco de dados (NoSql e/ou SQL)", model.ConhecimentoBancosDados);
                AddField(column, "Experiência com metodologias ágeis", model.ExperienciaMetodologiasAgeis);
                AddField(column, "Clean Code", model.CleanCode);
                AddField(column, "Vai precisar de equipamento para trabalhar ou tem preferência de usar o seu?", model.Equipamento);
            });
        }

        private void AddField(QuestPDF.Fluent.ColumnDescriptor column, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            column.Item().PaddingTop(15).Column(fieldColumn =>
            {
                fieldColumn.Item().Text(label)
                    .FontSize(11)
                    .Bold()
                    .FontColor(Colors.Grey.Darken3);

                fieldColumn.Item().PaddingTop(3).Text(value)
                    .FontSize(10)
                    .LineHeight(1.4f)
                    .FontColor(Colors.Grey.Darken2);
            });
        }

        public byte[] GeneratePdfFromMarkdown(string markdown)
        {
            try
            {
                // Converter Markdown para HTML usando Markdig
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();
                
                var html = Markdown.ToHtml(markdown, pipeline);

                // Adicionar estilos CSS básicos para melhor formatação
                var styledHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }}
        h1, h2, h3 {{
            color: #2c3e50;
            margin-top: 20px;
            margin-bottom: 10px;
        }}
        h1 {{
            font-size: 24px;
            border-bottom: 2px solid #3498db;
            padding-bottom: 10px;
        }}
        h2 {{
            font-size: 20px;
            border-bottom: 1px solid #ecf0f1;
            padding-bottom: 5px;
        }}
        h3 {{
            font-size: 16px;
        }}
        p {{
            margin-bottom: 10px;
        }}
        ul, ol {{
            margin-bottom: 15px;
            padding-left: 30px;
        }}
        li {{
            margin-bottom: 5px;
        }}
        strong {{
            color: #2c3e50;
        }}
        a {{
            color: #3498db;
            text-decoration: none;
        }}
        a:hover {{
            text-decoration: underline;
        }}
        code {{
            background-color: #f4f4f4;
            padding: 2px 6px;
            border-radius: 3px;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
        }}
        pre {{
            background-color: #f4f4f4;
            padding: 15px;
            border-radius: 5px;
            overflow-x: auto;
        }}
        blockquote {{
            border-left: 4px solid #3498db;
            padding-left: 15px;
            margin-left: 0;
            color: #7f8c8d;
        }}
    </style>
</head>
<body>
{html}
</body>
</html>";

                // Converter HTML para PDF usando DinkToPdf
                var converter = new SynchronizedConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 }
                    },
                    Objects = {
                        new ObjectSettings() {
                            HtmlContent = styledHtml,
                            WebSettings = { DefaultEncoding = "utf-8" }
                        }
                    }
                };

                var pdf = converter.Convert(doc);
                return pdf;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao converter Markdown em PDF", ex);
            }
        }
    }
}
