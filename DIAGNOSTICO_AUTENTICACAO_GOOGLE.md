# Diagnóstico e Correção - Autenticação Google OAuth

## 🔍 Problema Identificado
Falha na autenticação com Google OAuth. O usuário recebe mensagem "Falha na autenticação com Google. Tente novamente."

## 📋 Fluxo Atual de Autenticação

### 1. Início do Fluxo
```
Usuário clica "Entrar com Google" 
→ JavaScript chama: /api/auth/google-login
→ Controller redireciona para Google OAuth
```

### 2. Callback do Google
```
Google redireciona para: /api/auth/google-callback
→ Controller processa claims do Google
→ Cria/atualiza usuário no banco
→ Gera tokens JWT
→ Redireciona para: /index.html?auth=success&tokens=...
```

### 3. Processamento no Frontend
```
JavaScript detecta parâmetros na URL
→ Decodifica tokens (Base64)
→ Salva no localStorage
→ Atualiza UI
```

---

## 🐛 Possíveis Causas do Problema

### 1. **Configuração do Google Cloud Console**

**Verificar:**
- ✅ Client ID e Client Secret estão corretos no `appsettings.json`
- ✅ **URIs de redirecionamento autorizados** no Google Cloud Console:
  - `http://localhost:5083/api/auth/google-callback` (desenvolvimento)
  - `https://seu-dominio.com/api/auth/google-callback` (produção)
- ✅ **Origens JavaScript autorizadas:**
  - `http://localhost:5083` (desenvolvimento)
  - `https://seu-dominio.com` (produção)

**Como verificar:**
1. Acesse [Google Cloud Console](https://console.cloud.google.com/)
2. Vá em "APIs e Serviços" → "Credenciais"
3. Encontre seu OAuth 2.0 Client ID
4. Verifique as URIs de redirecionamento

---

### 2. **Problema com Cookies e SameSite**

**Problema:** Cookies podem ser bloqueados por políticas de SameSite em alguns navegadores.

**Solução:** Atualizar configuração no `Program.cs`:

```csharp
.AddCookie("Cookies", cookieOptions =>
{
    cookieOptions.Cookie.Name = "CurriculoInterativo.Auth";
    cookieOptions.Cookie.HttpOnly = true;
    cookieOptions.Cookie.SameSite = SameSiteMode.Lax; // ← Pode precisar ser None
    cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ← Pode precisar ser Always em HTTPS
    cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    cookieOptions.SlidingExpiration = true;
    cookieOptions.Cookie.Path = "/";
})
```

**Para produção (HTTPS):**
```csharp
cookieOptions.Cookie.SameSite = SameSiteMode.None;
cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
```

---

### 3. **Problema com Callback URL**

**Verificar no `AuthController.cs` linha 259:**
```csharp
var callbackUrl = $"{scheme}://{host}/api/auth/google-callback";
```

**Possíveis problemas:**
- Em produção com proxy reverso, o `Request.Scheme` pode estar errado
- O `Request.Host` pode não incluir a porta correta

**Solução:** Usar configuração explícita ou variável de ambiente:

```csharp
[HttpGet("google-login")]
[AllowAnonymous]
public IActionResult GoogleLogin()
{
    // Usar configuração explícita ou variável de ambiente
    var baseUrl = _configuration["AppSettings:BaseUrl"] 
        ?? $"{Request.Scheme}://{Request.Host}";
    
    var callbackUrl = $"{baseUrl}/api/auth/google-callback";
    
    var properties = new AuthenticationProperties 
    { 
        RedirectUri = callbackUrl,
        AllowRefresh = true,
        IsPersistent = false
    };
    
    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
}
```

---

### 4. **Problema com Claims do Google**

**Verificar no `AuthController.cs` linhas 313-319:**
O código tenta buscar claims de múltiplas formas, mas pode não estar encontrando.

**Solução melhorada:**
```csharp
// Tentar múltiplos tipos de claims
var googleId = claims.FirstOrDefault(c => 
    c.Type == ClaimTypes.NameIdentifier || 
    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" ||
    c.Type == "sub")?.Value;

var email = claims.FirstOrDefault(c => 
    c.Type == ClaimTypes.Email || 
    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" ||
    c.Type == "email")?.Value;

var name = claims.FirstOrDefault(c => 
    c.Type == ClaimTypes.Name || 
    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
    c.Type == "name")?.Value
    ?? email?.Split('@')[0] ?? "Usuário";
```

---

### 5. **Problema com Logs**

**Verificar logs do servidor:**
- Linha 310 do `AuthController.cs` já faz log de todas as claims
- Verificar se as claims estão sendo recebidas corretamente

**Adicionar mais logs:**
```csharp
_logger.LogInformation("Iniciando autenticação Google");
_logger.LogInformation("Callback URL: {CallbackUrl}", callbackUrl);
_logger.LogInformation("Scheme: {Scheme}, Host: {Host}", Request.Scheme, Request.Host);
```

---

## 🔧 Correções Recomendadas

### Correção 1: Melhorar Tratamento de Erros

**Arquivo:** `CurriculoInterativo.Api/Controllers/AuthController.cs`

Adicionar mais detalhes nos logs e mensagens de erro:

```csharp
[HttpGet("google-callback")]
[AllowAnonymous]
public async Task<IActionResult> GoogleCallback()
{
    try
    {
        _logger.LogInformation("Google callback recebido. Query: {Query}", Request.QueryString);
        
        // Verificar se há erro na query string
        if (Request.Query.ContainsKey("error"))
        {
            var error = Request.Query["error"].ToString();
            var errorDescription = Request.Query["error_description"].ToString();
            _logger.LogWarning("Erro retornado pelo Google: {Error} - {Description}", error, errorDescription);
            return Redirect($"/index.html?error=google_auth_failed&details={Uri.EscapeDataString(errorDescription ?? error)}");
        }

        // Autenticar com o esquema do Google
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            _logger.LogWarning("Falha na autenticação Google. Succeeded: {Succeeded}, Error: {Error}", 
                result.Succeeded, result.Failure?.Message);
            
            if (result.Failure != null)
            {
                _logger.LogError("Exception completa: {Exception}", result.Failure.ToString());
            }
            
            return Redirect($"/index.html?error=google_auth_failed&details={Uri.EscapeDataString(result.Failure?.Message ?? "Unknown error")}");
        }

        // ... resto do código
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro no callback do Google: {Message}. StackTrace: {StackTrace}", 
            ex.Message, ex.StackTrace);
        return Redirect($"/index.html?error=google_callback_error&details={Uri.EscapeDataString(ex.Message)}");
    }
}
```

---

### Correção 2: Adicionar Configuração de Base URL

**Arquivo:** `CurriculoInterativo.Api/appsettings.json`

Adicionar:
```json
{
  "AppSettings": {
    "BaseUrl": "https://seu-dominio.com"
  }
}
```

**Para desenvolvimento local, deixar vazio ou:**
```json
{
  "AppSettings": {
    "BaseUrl": "http://localhost:5083"
  }
}
```

---

### Correção 3: Melhorar Frontend - Mostrar Erros Detalhados

**Arquivo:** `CurriculoInterativo.Api/wwwroot/js/app.js`

Melhorar função `handleGoogleCallback()`:

```javascript
function handleGoogleCallback() {
    const urlParams = new URLSearchParams(window.location.search);
    const authStatus = urlParams.get('auth');
    const tokensParam = urlParams.get('tokens');
    const error = urlParams.get('error');
    const details = urlParams.get('details');

    if (error) {
        let errorMessage = 'Erro ao fazer login com Google.';
        switch (error) {
            case 'google_auth_failed':
                errorMessage = 'Falha na autenticação com Google.';
                if (details) {
                    errorMessage += `\n\nDetalhes: ${decodeURIComponent(details)}`;
                }
                break;
            case 'google_info_incomplete':
                errorMessage = 'Informações do Google incompletas. Tente novamente.';
                break;
            case 'google_callback_error':
                errorMessage = 'Erro no processamento do login.';
                if (details) {
                    errorMessage += `\n\nDetalhes: ${decodeURIComponent(details)}`;
                }
                break;
        }
        
        // Usar modal em vez de alert para melhor UX
        showErrorModal(errorMessage);
        
        // Limpar URL
        window.history.replaceState({}, document.title, window.location.pathname);
        return;
    }

    // ... resto do código
}

function showErrorModal(message) {
    // Criar modal de erro mais amigável
    const modal = document.createElement('div');
    modal.className = 'error-modal';
    modal.innerHTML = `
        <div class="error-modal-content">
            <h3><i class="fas fa-exclamation-triangle"></i> Erro no Login</h3>
            <p>${message}</p>
            <button onclick="this.closest('.error-modal').remove()">Fechar</button>
        </div>
    `;
    document.body.appendChild(modal);
}
```

---

### Correção 4: Verificar Configuração do Google OAuth

**Checklist:**

1. ✅ **Google Cloud Console:**
   - [ ] OAuth 2.0 Client ID está ativo
   - [ ] Client ID e Secret estão corretos no `appsettings.json`
   - [ ] URIs de redirecionamento incluem:
     - `http://localhost:5083/api/auth/google-callback` (dev)
     - `https://seu-dominio.com/api/auth/google-callback` (prod)
   - [ ] Origens JavaScript autorizadas incluem:
     - `http://localhost:5083` (dev)
     - `https://seu-dominio.com` (prod)

2. ✅ **appsettings.json:**
   ```json
   "GoogleOAuth": {
     "ClientId": "SEU_CLIENT_ID.apps.googleusercontent.com",
     "ClientSecret": "SEU_CLIENT_SECRET"
   }
   ```

3. ✅ **Program.cs:**
   - CallbackPath: `/api/auth/google-callback`
   - SignInScheme: `"Cookies"`

---

## 🧪 Testes de Diagnóstico

### Teste 1: Verificar se o endpoint está acessível
```bash
curl http://localhost:5083/api/auth/google-login
```
**Esperado:** Redirecionamento 302 para Google

### Teste 2: Verificar logs do servidor
Ao tentar fazer login, verificar:
- Logs de "Google callback recebido"
- Logs de "Claims recebidas do Google"
- Erros específicos

### Teste 3: Verificar no navegador
1. Abrir DevTools (F12)
2. Ir em Network
3. Tentar fazer login
4. Verificar requisições e respostas
5. Verificar cookies criados

---

## 🚀 Solução Rápida (Troubleshooting)

### Passo 1: Verificar Logs
```bash
# Ver logs do servidor ao tentar login
# Procurar por:
# - "Google callback recebido"
# - "Claims recebidas do Google"
# - "Erro no OAuth do Google"
```

### Passo 2: Verificar Configuração
```bash
# Verificar se ClientId e ClientSecret estão corretos
cat appsettings.json | grep -A 2 GoogleOAuth
```

### Passo 3: Testar Callback URL Manualmente
No Google Cloud Console, verificar se a URL de callback está exatamente:
```
http://localhost:5083/api/auth/google-callback
```
(ou a URL de produção correspondente)

### Passo 4: Limpar Cookies
```javascript
// No console do navegador:
document.cookie.split(";").forEach(c => {
    document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
});
```

---

## 📝 Checklist de Correção

- [ ] Verificar Client ID e Secret no Google Cloud Console
- [ ] Verificar URIs de redirecionamento no Google Cloud Console
- [ ] Verificar configuração no `appsettings.json`
- [ ] Verificar logs do servidor ao tentar login
- [ ] Testar em modo anônimo/privado do navegador
- [ ] Verificar se cookies estão sendo criados
- [ ] Verificar se está usando HTTPS em produção (obrigatório para SameSite=None)
- [ ] Adicionar mais logs para diagnóstico
- [ ] Melhorar mensagens de erro no frontend

---

## 🔗 Links Úteis

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core Google Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins)
- [Google Cloud Console](https://console.cloud.google.com/)

---

**Última atualização:** 2025-01-30
