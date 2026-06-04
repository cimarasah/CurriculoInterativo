namespace CurriculoInterativo.Api.DTOs.JobApplicationDto
{
    public class DedicatedCurriculumModel
    {
        public string? CompanyName { get; set; }
        public string Candidato { get; set; } = string.Empty;
        public string Residencia { get; set; } = string.Empty;
        public string Graduacao { get; set; } = string.Empty;
        public string AtuacaoEmpresasFinanceiras { get; set; } = string.Empty;
        public string TempoComoDev { get; set; } = string.Empty;
        public string TempoComoDotNet { get; set; } = string.Empty;
        public string TempoComJava { get; set; } = string.Empty;
        public string PrincipaisTecnologias { get; set; } = string.Empty;
        public string Mensageria { get; set; } = string.Empty;
        public string ApiDesignRestfulMicroservices { get; set; } = string.Empty;
        public string ConhecimentoCloud { get; set; } = string.Empty;
        public string DockerKubernetes { get; set; } = string.Empty;
        public string EsteirasCICD { get; set; } = string.Empty;
        public string Serverless { get; set; } = string.Empty;
        public string ConhecimentoBancosDados { get; set; } = string.Empty;
        public string ExperienciaMetodologiasAgeis { get; set; } = string.Empty;
        public string CleanCode { get; set; } = string.Empty;
        public string Equipamento { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}

