using CurriculoInterativo.Api.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
    }
}
