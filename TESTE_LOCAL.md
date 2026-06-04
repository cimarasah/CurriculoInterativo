# Teste Local

## Pré-requisitos

1. API Key do Google Gemini configurada no `appsettings.json`
2. Banco de dados SQL Server acessível
3. Dados do currículo cadastrados no banco
4. Google OAuth configurado no Google Cloud Console

## Iniciar Aplicação

```bash
cd CurriculoInterativo.Api
dotnet run
```

A aplicação inicia em: `http://localhost:5083`

## Login com Google

1. Clique em "Login" > "Entrar com Google"
2. Selecione sua conta Google e autorize
3. Você será redirecionado e estará logado

**Importante**: Funcionalidades que usam IA requerem autenticação.

## Testar Currículo Dedicado

1. Acesse a seção "Currículo Dedicado à Vaga"
2. Preencha:
   - **Nome da Empresa** (opcional)
   - **Descrição da Vaga** (obrigatório, mínimo 50 caracteres)
3. Clique em "Gerar Currículo Personalizado"
4. Aguarde o processamento (30-60 segundos)
5. O PDF será baixado automaticamente

## Exemplo de Descrição de Vaga

```
Desenvolvedor .NET Senior

Requisitos:
- Experiência sólida em desenvolvimento .NET (C#)
- Conhecimento em ASP.NET Core, Entity Framework
- Experiência com APIs RESTful e Microservices
- Conhecimento em bancos de dados SQL Server
- Experiência com Docker e Kubernetes
- Conhecimento em Cloud (preferencialmente AWS)
- Experiência com metodologias ágeis (Scrum, Kanban)
- Clean Code e boas práticas de desenvolvimento
- Experiência com CI/CD

Diferenciais:
- Experiência com Java
- Conhecimento em mensageria (RabbitMQ, Kafka)
- Experiência com Serverless
- Conhecimento em NoSQL
```

## Troubleshooting

### Erro: "Você precisa estar autenticado"
- Faça login com Google primeiro

### Erro: "A descrição da vaga deve ter pelo menos 50 caracteres"
- Cole uma descrição mais completa

### Erro: "Erro ao processar a pergunta"
- Verifique a API Key do Google Gemini no `appsettings.json`
- Verifique os logs do console
- Verifique o uso no Google Cloud Console

### Erro: "oauth state was missing or invalid"
- Limpe os cookies do navegador
- Verifique se o callback URL está configurado no Google Cloud Console:
  - Desenvolvimento: `http://localhost:5083/api/auth/google-callback`
  - Produção: `https://seu-dominio.com/api/auth/google-callback`

### A seção não aparece
- Verifique se está logado
- Verifique o console do navegador (F12) para erros JavaScript

## Verificar Logs

Os logs aparecem no terminal onde a aplicação está rodando:
- `Iniciando geração de currículo dedicado...`
- `Chamando IA para pergunta: [nome do campo]`
- `Resposta da IA gerada para pergunta: [nome do campo]`
- `Geração de currículo dedicado concluída.`

## Teste via Swagger

1. Acesse: `http://localhost:5083/swagger`
2. Expanda `POST /api/DedicatedCurriculum/generate`
3. Clique em "Try it out"
4. Clique no botão de autorização (🔒) e adicione seu token JWT
5. Preencha o JSON e execute

