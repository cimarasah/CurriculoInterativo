using CurriculoInterativo.Api.Services.TokenService;

namespace CurriculoInterativo.Api.Services.BackgroundServices
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(6); // Executa a cada 6 horas

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                    await tokenService.CleanupExpiredTokensAsync();
                    _logger.LogInformation("Limpeza de tokens expirados executada com sucesso em {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a limpeza de tokens expirados");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}
