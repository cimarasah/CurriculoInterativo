using System.Net;

namespace CurriculoInterativo.Api.Utils.Helpers
{
    public class IpAddressHelper
    {
        /// <summary>
        /// Obtém o endereço IP real do cliente, considerando proxies e load balancers
        /// </summary>
        /// <param name="httpContext">Contexto HTTP da requisição</param>
        /// <returns>Endereço IP do cliente</returns>
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            try
            {
                // Lista de headers que podem conter o IP real (em ordem de prioridade)
                var ipHeaders = new[]
                {
                    "X-Forwarded-For",      // Padrão para proxies
                    "X-Real-IP",            // Nginx
                    "CF-Connecting-IP",     // Cloudflare
                    "True-Client-IP",       // Cloudflare Enterprise
                    "X-Client-IP",          // Apache
                    "X-Cluster-Client-IP",  // Outros proxies
                    "Forwarded"             // RFC 7239
                };

                // Tentar obter IP dos headers
                foreach (var header in ipHeaders)
                {
                    if (httpContext.Request.Headers.TryGetValue(header, out var headerValue))
                    {
                        var ipValue = headerValue.FirstOrDefault();

                        if (!string.IsNullOrEmpty(ipValue))
                        {
                            // X-Forwarded-For pode conter múltiplos IPs separados por vírgula
                            // O primeiro é o IP real do cliente
                            var ips = ipValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

                            foreach (var ip in ips)
                            {
                                var trimmedIp = ip.Trim();

                                // Validar se é um IP válido
                                if (IsValidIpAddress(trimmedIp))
                                {
                                    return trimmedIp;
                                }
                            }
                        }
                    }
                }

                // Se não encontrou nos headers, usar RemoteIpAddress
                var remoteIp = httpContext.Connection.RemoteIpAddress;
                if (remoteIp != null)
                {
                    // Converter IPv6 localhost para IPv4
                    if (remoteIp.IsIPv4MappedToIPv6)
                    {
                        remoteIp = remoteIp.MapToIPv4();
                    }

                    return remoteIp.ToString();
                }

                // Fallback
                return "0.0.0.0";
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }

        /// <summary>
        /// Valida se uma string é um endereço IP válido
        /// </summary>
        private static bool IsValidIpAddress(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
                return false;

            // Remover porta se existir (ex: 192.168.1.1:8080)
            var ipWithoutPort = ipString.Split(':')[0];

            // Tentar fazer parse
            if (!IPAddress.TryParse(ipWithoutPort, out var ipAddress))
                return false;

            // Ignorar IPs locais em produção (opcional)
            // return !IsPrivateIP(ipAddress);

            return true;
        }

        /// <summary>
        /// Verifica se o IP é privado/local (opcional)
        /// </summary>
        private static bool IsPrivateIP(IPAddress ipAddress)
        {
            if (IPAddress.IsLoopback(ipAddress))
                return true;

            var bytes = ipAddress.GetAddressBytes();

            // 10.0.0.0 - 10.255.255.255
            if (bytes[0] == 10)
                return true;

            // 172.16.0.0 - 172.31.255.255
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;

            // 192.168.0.0 - 192.168.255.255
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;

            return false;
        }

        /// <summary>
        /// Obtém informações detalhadas sobre a requisição (para debug)
        /// </summary>
        public static string GetRequestInfo(HttpContext httpContext)
        {
            var info = new System.Text.StringBuilder();

            info.AppendLine($"IP: {GetClientIpAddress(httpContext)}");
            info.AppendLine($"User-Agent: {httpContext.Request.Headers["User-Agent"]}");
            info.AppendLine($"Referer: {httpContext.Request.Headers["Referer"]}");

            // Headers relacionados a IP
            info.AppendLine("IP Headers:");
            foreach (var header in httpContext.Request.Headers.Where(h =>
                h.Key.Contains("IP", StringComparison.OrdinalIgnoreCase) ||
                h.Key.Contains("Forward", StringComparison.OrdinalIgnoreCase)))
            {
                info.AppendLine($"  {header.Key}: {header.Value}");
            }

            return info.ToString();
        }
    }
}
