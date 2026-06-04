namespace CurriculoInterativo.Api.Services.EmailService
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia um email
        /// </summary>
        /// <param name="to">Email do destinatário</param>
        /// <param name="subject">Assunto do email</param>
        /// <param name="body">Corpo do email (HTML ou texto)</param>
        /// <param name="isHtml">Indica se o corpo é HTML</param>
        /// <returns>True se enviado com sucesso</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
}

