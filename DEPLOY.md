# Deploy

## Azure Static Web Apps

### Criar Static Web App

1. Azure Portal > Criar > Static Web Apps
2. Preencha:
   - **Nome**: `curriculo-interativo`
   - **Fonte**: GitHub
   - **App location**: `CurriculoInterativo.Api/wwwroot`
   - **Api location**: `CurriculoInterativo.Api`
   - **Output location**: `output/wwwroot`

### Configurar Variáveis de Ambiente

No Azure Portal > Configuration > Application settings:

```
GoogleGeminiSettings__ApiKey = [sua-api-key]
GoogleGeminiSettings__ModelName = gemini-pro
GoogleOAuth__ClientId = [seu-client-id]
GoogleOAuth__ClientSecret = [seu-client-secret]
ConnectionStrings__ResumeDb = [sua-connection-string]
```

**Importante**: Use `__` (dois underscores) para separar seções.

### Workflow GitHub Actions

O Azure criará automaticamente o workflow em `.github/workflows/azure-static-web-apps-*.yml`

## Azure App Service (Recomendado)

### Criar App Service

1. Azure Portal > Criar > App Service
2. Preencha:
   - **Nome**: `curriculo-interativo-api`
   - **Runtime stack**: .NET 8
   - **OS**: Windows ou Linux

### Configurar Variáveis de Ambiente

No Azure Portal > Configuration > Application settings:

```
GoogleGeminiSettings__ApiKey = [sua-api-key]
GoogleGeminiSettings__ModelName = gemini-pro
GoogleOAuth__ClientId = [seu-client-id]
GoogleOAuth__ClientSecret = [seu-client-secret]
JwtSettings__SecretKey = [sua-secret-key]
JwtSettings__Issuer = CurriculoInterativo.Api
JwtSettings__Audience = CurriculoInterativo.Client
JwtSettings__ExpirationHours = 1
```

### Connection String

No Azure Portal > Configuration > Connection strings:

- **Nome**: `ResumeDb`
- **Valor**: `[sua-connection-string]`
- **Tipo**: `SQLAzure`

### Deploy via GitHub Actions

1. Baixe o Publish Profile do App Service
2. No GitHub > Settings > Secrets > Actions, adicione:
   - **Nome**: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - **Valor**: Conteúdo do arquivo `.PublishSettings`
3. O workflow `.github/workflows/azure-app-service-deploy.yml` fará deploy automaticamente

### Atualizar URL da API no Frontend

No arquivo `wwwroot/js/app.js`:

```javascript
const API_BASE_URL = window.location.origin + '/api'; // Se frontend e backend estão juntos
// OU
const API_BASE_URL = 'https://curriculo-interativo-api.azurewebsites.net/api'; // Se separados
```

## Google Cloud Console

Configure o callback URL:

- **Desenvolvimento**: `http://localhost:5083/api/auth/google-callback`
- **Produção**: `https://seu-dominio.com/api/auth/google-callback`

## Troubleshooting

### Erro: "No package found"
- Verifique se o caminho `./CurriculoInterativo.Api` está correto

### Erro: "Publish profile not found"
- Verifique se o secret `AZURE_WEBAPP_PUBLISH_PROFILE` foi adicionado

### Variáveis de ambiente não funcionam
- Use `__` (dois underscores) para separar seções
- Exemplo: `GoogleGeminiSettings__ApiKey`

## Checklist

- [ ] App Service/Static Web App criado
- [ ] Variáveis de ambiente configuradas
- [ ] Connection string configurada
- [ ] Publish Profile baixado (App Service)
- [ ] Secret adicionado no GitHub (App Service)
- [ ] Workflow criado no GitHub
- [ ] URL da API atualizada no frontend
- [ ] Callback URL configurado no Google Cloud Console
- [ ] Deploy verificado na aba Actions

