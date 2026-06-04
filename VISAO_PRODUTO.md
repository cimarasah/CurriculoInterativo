# 📋 Visão de Produto - Currículo Interativo API

**Documento de Visão de Produto**  
**Versão:** 1.0  
**Data:** 2024  
**Gerente de Produto:** Análise Estrutural

---

## 🎯 1. Visão Executiva

### 1.1. O Que É
O **Currículo Interativo API** é uma plataforma inovadora que transforma o currículo tradicional em uma experiência digital interativa e dinâmica. Em vez de enviar um PDF estático, o produto oferece uma API RESTful completa que permite recrutadores explorarem habilidades, projetos, certificações e experiências profissionais de forma autônoma e intuitiva.

### 1.2. Proposta de Valor
- **Para Recrutadores:** Acesso dinâmico e filtrado a informações profissionais, economia de tempo na triagem, visualização organizada por categorias
- **Para o Candidato (Owner):** Demonstração prática de habilidades técnicas, diferenciação no mercado, atualização fácil e centralizada do portfólio
- **Diferencial Competitivo:** Primeiro currículo do mercado apresentado como API funcional com IA integrada para personalização

### 1.3. Posicionamento no Mercado
**Categoria:** Portfólio Profissional / Personal Branding Tech  
**Segmento:** Desenvolvedores de Software Sênior  
**Modelo:** B2C (Business to Candidate) - Foco em recrutadores e empresas

---

## 👥 2. Público-Alvo e Personas

### 2.1. Persona Primária: Recrutadores Tech

**Perfil:**
- Profissionais de RH/Talent Acquisition em empresas de tecnologia
- Tech Recruiters especializados em .NET, C#, Backend
- Headhunters buscando desenvolvedores sênior
- Líderes técnicos avaliando candidatos

**Necessidades:**
- Acesso rápido a informações relevantes
- Filtragem por tecnologias específicas
- Visualização de projetos e certificações
- Entendimento do nível de proficiência
- Download de currículo em PDF quando necessário

**Jornada do Usuário:**
1. Recebe link do currículo interativo
2. Explora seções públicas (projetos, habilidades, experiências)
3. Filtra projetos por tecnologia de interesse
4. Visualiza certificações por categoria
5. Baixa PDF padrão ou solicita currículo personalizado (se autenticado)

### 2.2. Persona Secundária: Owner (Candidato)

**Perfil:**
- Desenvolvedor .NET Sênior (Cimara Sá)
- Busca demonstrar habilidades técnicas através do próprio produto
- Necessita atualizar informações regularmente
- Quer métricas de engajamento

**Necessidades:**
- Gerenciamento fácil de dados do currículo
- Autenticação segura para edição
- Geração de currículos personalizados por vaga usando IA
- Estatísticas de acesso e downloads
- Segurança e privacidade dos dados

---

## 🚀 3. Funcionalidades Principais

### 3.1. Acesso Público (Sem Autenticação)

#### 3.1.1. Visualização de Experiências Profissionais
- **Endpoint:** `GET /api/experience`
- **Descrição:** Lista completa de histórico profissional
- **Valor:** Recrutadores veem trajetória e evolução da carreira
- **Métricas:** Visualizações, tempo médio na seção

#### 3.1.2. Catálogo de Projetos
- **Endpoint:** `GET /api/project`
- **Filtro por Tecnologia:** `GET /api/project/skill/{skill}`
- **Descrição:** Projetos com tecnologias associadas e responsabilidades
- **Valor:** Entendimento prático das habilidades aplicadas
- **Diferencial:** Filtragem dinâmica permite encontrar projetos específicos

#### 3.1.3. Certificações Organizadas
- **Endpoint:** `GET /api/certification`
- **Descrição:** Certificações filtradas por categoria (Backend, Frontend, Cloud, etc.)
- **Valor:** Validação de conhecimentos por entidades reconhecidas
- **Organização:** Categorização facilita navegação

#### 3.1.4. Habilidades Técnicas
- **Endpoint:** `GET /api/skill`
- **Descrição:** Skills com nível de proficiência e tempo de experiência
- **Valor:** Entendimento claro do domínio técnico
- **Dados:** Nível (Básico, Intermediário, Avançado, Expert) + anos de experiência

#### 3.1.5. Informações de Contato
- **Endpoint:** `GET /api/contact`
- **Descrição:** Todas as formas de contato (LinkedIn, GitHub, Email, etc.)
- **Valor:** Facilita próximo passo na jornada de recrutamento

#### 3.1.6. Download de Currículo Padrão
- **Endpoint:** `GET /api/curriculum/download`
- **Descrição:** Geração e download de PDF com todos os dados
- **Valor:** Formato tradicional quando necessário
- **Tecnologia:** QuestPDF para geração

### 3.2. Acesso Autenticado (Owner)

#### 3.2.1. Autenticação Multi-Provider
- **Endpoints:** 
  - `POST /api/auth/login` (tradicional)
  - `GET /api/auth/google` (OAuth Google)
  - `POST /api/auth/refresh` (renovação de token)
  - `POST /api/auth/logout` (revogação)
- **Tecnologia:** JWT Bearer Tokens + Refresh Tokens
- **Segurança:** 
  - Hashing de senhas (bcrypt)
  - Tokens com expiração
  - Revogação de refresh tokens
- **Valor:** Acesso seguro e flexível

#### 3.2.2. CRUD Completo de Entidades
- **Projetos:** Criar, ler, atualizar, deletar
- **Experiências:** Gerenciamento completo
- **Certificações:** Adicionar novas, atualizar existentes
- **Habilidades:** Manter skills atualizadas
- **Contatos:** Atualizar informações de contato
- **Valor:** Manutenção centralizada e fácil

#### 3.2.3. Currículo Dedicado à Vaga (IA)
- **Endpoint:** `POST /api/DedicatedCurriculum/generate`
- **Descrição:** Geração de currículo personalizado usando IA (Google Gemini)
- **Input:** Nome da empresa (opcional) + Descrição da vaga (obrigatório, min 50 caracteres)
- **Processo:**
  1. Sistema analisa descrição da vaga
  2. IA extrai informações relevantes do currículo base
  3. Gera respostas personalizadas para cada seção
  4. Cria PDF otimizado para a vaga específica
- **Tempo de Processamento:** 30-60 segundos
- **Valor:** Currículo 100% alinhado com requisitos da vaga
- **Diferencial Competitivo:** Único no mercado com IA integrada

#### 3.2.4. Sistema de Sugestões
- **Endpoint:** `GET /api/suggestion` (público), CRUD completo (autenticado)
- **Descrição:** Recrutadores podem deixar sugestões de melhorias
- **Valor:** Feedback contínuo e melhoria do produto

#### 3.2.5. Estatísticas e Métricas (Roadmap)
- **Endpoint:** `GET /api/curriculum/stats` (Owner only)
- **Descrição:** Dashboard de métricas (em desenvolvimento)
- **Métricas Planejadas:**
  - Downloads de PDF
  - Visualizações por seção
  - Filtros mais utilizados
  - IPs e User-Agents (para análise geográfica)
  - Taxa de conversão (visualização → contato)

---

## 🏗️ 4. Arquitetura e Tecnologias

### 4.1. Stack Tecnológico

| Camada | Tecnologias | Justificativa |
|--------|-------------|---------------|
| **Backend** | .NET 8, ASP.NET Core Web API | Performance, segurança, ecossistema robusto |
| **ORM** | Entity Framework Core 8.10 | Code-first migrations, produtividade |
| **Banco de Dados** | SQL Server (Azure SQL Database) | Confiabilidade, escalabilidade, integração Azure |
| **Autenticação** | JWT Bearer + OAuth 2.0 (Google) | Padrão da indústria, segurança, UX |
| **Mapeamento** | AutoMapper 13.0.1 | Separação de concerns, manutenibilidade |
| **IA** | Google Gemini API | Personalização inteligente de currículos |
| **PDF** | QuestPDF 2025.7.2 | Geração programática de documentos |
| **Email** | MailKit 4.14.1 | Notificações e recuperação de senha |
| **Documentação** | Swagger/OpenAPI | Facilita integração e testes |

### 4.2. Arquitetura de Software

**Padrão:** Clean Architecture / Repository Pattern

```
CurriculoInterativo.Api/
├── Controllers/        # Camada de apresentação (HTTP)
├── DTOs/              # Objetos de transferência (desacoplamento)
├── Entities/          # Modelos de domínio (banco de dados)
├── Models/            # Modelos de negócio (lógica)
├── Services/          # Lógica de negócio e orquestração
├── Repositories/      # Acesso a dados (abstração)
├── Mapping/           # AutoMapper profiles
├── Utils/             # Helpers, Exceptions, Filters
└── Migrations/        # Evolução do schema (EF Core)
```

**Princípios:**
- **Separation of Concerns:** Cada camada tem responsabilidade única
- **Dependency Injection:** Baixo acoplamento, alta testabilidade
- **Repository Pattern:** Abstração de acesso a dados
- **DTO Pattern:** Proteção de modelos internos

### 4.3. Segurança

**Implementações:**
- ✅ JWT Tokens com expiração configurável
- ✅ Refresh Tokens com rotação
- ✅ Password Hashing (bcrypt via Identity)
- ✅ Role-based Authorization (Owner vs. Público)
- ✅ Input Validation (Data Annotations + FluentValidation)
- ✅ Global Exception Handling
- ✅ CORS configurado para origens específicas
- ✅ HTTPS enforcement (produção)
- ✅ SQL Injection protection (EF Core parameterized queries)

---

## 📊 5. Modelo de Dados

### 5.1. Entidades Principais

**User**
- Autenticação e autorização
- Suporte a múltiplos providers (Google OAuth + tradicional)
- Roles (Owner, User)

**Experience**
- Histórico profissional completo
- Períodos, empresas, cargos
- Relacionamento com Skills e Projects

**Project**
- Portfólio de projetos
- Tecnologias associadas (many-to-many com Skill)
- Responsabilidades e descrições

**Skill**
- Habilidades técnicas
- Nível de proficiência (Enum)
- Categoria (Backend, Frontend, Cloud, etc.)
- Tempo de experiência

**Certification**
- Certificações profissionais
- Categoria e data de obtenção
- Validação de conhecimentos

**Contact**
- Informações de contato
- LinkedIn, GitHub, Email, etc.

**Suggestion**
- Feedback de recrutadores
- Melhorias sugeridas

**RefreshToken / PasswordResetToken**
- Gestão de segurança e recuperação

### 5.2. Relacionamentos

- User ↔ Experience (1:N)
- Project ↔ Skill (N:N)
- User ↔ Certification (1:N)
- User ↔ Contact (1:1)

---

## ☁️ 6. Infraestrutura e Deploy

### 6.1. Ambiente de Produção

**Plataforma:** Microsoft Azure

| Componente | Serviço Azure | Justificativa |
|------------|---------------|---------------|
| **API Backend** | Azure App Service | Escalabilidade automática, integração nativa |
| **Banco de Dados** | Azure SQL Database | Gerenciado, backups automáticos, alta disponibilidade |
| **Storage** | Azure Blob Storage (futuro) | Armazenamento de PDFs gerados |
| **CDN** | Azure CDN (futuro) | Performance global |
| **CI/CD** | Azure DevOps (roadmap) | Automação de deploy |

### 6.2. Configurações de Ambiente

**Variáveis Necessárias:**
- `ConnectionStrings__ResumeDb` - String de conexão SQL Server
- `JwtSettings__SecretKey` - Chave secreta para JWT
- `JwtSettings__Issuer` - Emissor do token
- `JwtSettings__Audience` - Audiência do token
- `JwtSettings__ExpirationHours` - Tempo de expiração
- `GoogleGeminiSettings__ApiKey` - API Key do Google Gemini
- `GoogleGeminiSettings__ModelName` - Modelo de IA (gemini-pro)
- `GoogleOAuth__ClientId` - OAuth Client ID
- `GoogleOAuth__ClientSecret` - OAuth Client Secret

### 6.3. Monitoramento (Roadmap)

**Métricas Planejadas:**
- Uptime e disponibilidade
- Tempo de resposta de endpoints
- Taxa de erro (4xx, 5xx)
- Uso de recursos (CPU, memória)
- Queries lentas no banco
- Uso de API do Google Gemini (custos)

**Ferramentas Sugeridas:**
- Azure Application Insights
- Azure Monitor
- Log Analytics

---

## 📈 7. Roadmap e Evoluções Futuras

### 7.1. Fase Atual (v1.0) ✅

- [x] API RESTful completa
- [x] Autenticação JWT + OAuth Google
- [x] CRUD de todas as entidades
- [x] Geração de PDF padrão
- [x] Currículo dedicado com IA
- [x] Frontend básico integrado
- [x] Deploy no Azure

### 7.2. Fase 2 (v1.1) - Métricas e Analytics 🚧

**Prioridade:** Alta  
**Prazo Estimado:** 1-2 meses

- [ ] Dashboard de estatísticas para Owner
- [ ] Tracking de visualizações por seção
- [ ] Análise de downloads de PDF
- [ ] Heatmap de interações
- [ ] Relatórios de engajamento
- [ ] Exportação de métricas (CSV/Excel)

**Valor de Negócio:** Entender comportamento de recrutadores, otimizar conteúdo

### 7.3. Fase 3 (v1.2) - Melhorias de UX/UI 🎨

**Prioridade:** Média  
**Prazo Estimado:** 2-3 meses

- [ ] Redesign do frontend (React/Vue.js)
- [ ] Interface de gerenciamento (Admin Panel)
- [ ] Preview em tempo real de edições
- [ ] Upload de imagens para projetos
- [ ] Temas personalizáveis
- [ ] Modo escuro/claro

**Valor de Negócio:** Melhor experiência para Owner e Recrutadores

### 7.4. Fase 4 (v2.0) - Multi-User e White Label 🏢

**Prioridade:** Baixa (futuro)  
**Prazo Estimado:** 4-6 meses

- [ ] Suporte a múltiplos usuários (SaaS)
- [ ] White label (empresas podem usar para seus candidatos)
- [ ] Planos e assinaturas
- [ ] API pública para integrações
- [ ] Webhooks para eventos
- [ ] Marketplace de templates

**Valor de Negócio:** Expansão do modelo de negócio, monetização

### 7.5. Fase 5 (v2.1) - IA Avançada 🤖

**Prioridade:** Média  
**Prazo Estimado:** 3-4 meses

- [ ] Análise automática de descrição de vaga (extração de requisitos)
- [ ] Sugestões de otimização de currículo baseadas em IA
- [ ] Matching score (quanto o candidato se encaixa na vaga)
- [ ] Geração automática de cover letter
- [ ] Chatbot para dúvidas de recrutadores

**Valor de Negócio:** Diferenciação competitiva, automação

---

## 🎯 8. Métricas de Sucesso (KPIs)

### 8.1. Métricas de Engajamento

| Métrica | Meta | Atual | Status |
|---------|------|-------|--------|
| **Downloads de PDF** | 50/mês | - | 🚧 Em implementação |
| **Visualizações de Projetos** | 200/mês | - | 🚧 Em implementação |
| **Uso de Filtros** | 30% dos acessos | - | 🚧 Em implementação |
| **Gerações de Currículo Dedicado** | 10/mês | - | ✅ Funcional |
| **Tempo médio na plataforma** | >2 min | - | 🚧 Em implementação |

### 8.2. Métricas Técnicas

| Métrica | Meta | Status |
|---------|------|--------|
| **Uptime** | >99.5% | ✅ Azure App Service |
| **Tempo de resposta (p95)** | <500ms | 🚧 Monitorar |
| **Taxa de erro** | <1% | 🚧 Monitorar |
| **Cobertura de testes** | >70% | 🚧 Roadmap |

### 8.3. Métricas de Negócio

| Métrica | Meta | Status |
|---------|------|--------|
| **Taxa de conversão** (visualização → contato) | >5% | 🚧 Em implementação |
| **NPS** (Net Promoter Score) | >50 | 🚧 Roadmap |
| **Custo por geração de currículo** | <$0.10 | ✅ Otimizado |

---

## 🛡️ 9. Riscos e Mitigações

### 9.1. Riscos Técnicos

| Risco | Impacto | Probabilidade | Mitigação |
|-------|---------|---------------|-----------|
| **Custo elevado de API Gemini** | Alto | Média | Rate limiting, cache de respostas, monitoramento de uso |
| **Downtime do Azure** | Alto | Baixa | Multi-region deployment (futuro), health checks |
| **Vazamento de dados** | Crítico | Baixa | Criptografia, auditoria, testes de segurança |
| **Performance degradada** | Médio | Média | Cache, otimização de queries, CDN |

### 9.2. Riscos de Produto

| Risco | Impacto | Probabilidade | Mitigação |
|-------|---------|---------------|-----------|
| **Baixa adoção por recrutadores** | Alto | Média | Marketing, parcerias, melhorias de UX |
| **Concorrência** | Médio | Alta | Diferenciação (IA), inovação contínua |
| **Manutenção complexa** | Médio | Baixa | Documentação, testes, arquitetura limpa |

---

## 💰 10. Modelo de Negócio (Futuro)

### 10.1. Modelo Atual
- **Gratuito:** Uso pessoal do Owner
- **Custo:** Infraestrutura Azure (paga pelo Owner)

### 10.2. Modelo Futuro (SaaS)

**Plano Free:**
- 1 currículo
- 10 gerações de PDF/mês
- Suporte básico

**Plano Pro ($9.99/mês):**
- Currículos ilimitados
- Gerações ilimitadas
- Métricas avançadas
- Suporte prioritário

**Plano Enterprise (sob consulta):**
- White label
- API dedicada
- SLA garantido
- Suporte 24/7

---

## 📝 11. Conclusão

O **Currículo Interativo API** é um produto inovador que demonstra habilidades técnicas através da própria implementação. Com arquitetura sólida, segurança robusta e diferenciação competitiva (IA integrada), o produto está posicionado para:

1. **Demonstrar expertise técnica** do candidato
2. **Facilitar processo de recrutamento** para empresas
3. **Evoluir para plataforma SaaS** no futuro

**Próximos Passos Críticos:**
1. Implementar dashboard de métricas (Fase 2)
2. Melhorar UX/UI do frontend (Fase 3)
3. Expandir funcionalidades de IA (Fase 5)
4. Considerar modelo SaaS (Fase 4)

---

**Documento mantido por:** Equipe de Produto  
**Última atualização:** 2024  
**Próxima revisão:** Trimestral
