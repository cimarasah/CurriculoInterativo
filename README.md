# 💼 Curriculo Interativo - API Backend

> **Um currículo que respira código!** Transformei minha trajetória profissional em uma API RESTful completa, onde recrutadores podem explorar minhas habilidades, projetos e experiências de forma dinâmica e interativa.

## 🎯 O Conceito

Por que enviar um PDF estático quando posso oferecer uma **experiência imersiva**? Esta API serve como backend para um sistema onde:

- **Recrutadores** exploram livremente meu portfólio
- **Filtram projetos** por tecnologia utilizada
- **Navegam por certificações** organizadas por categoria
- **Acessam informações** de forma autônoma e intuitiva
- **Eu (Owner)** mantenho tudo atualizado com autenticação segura

## 🚀 Funcionalidades

### 🔓 Acesso Público (Recrutadores)

- **📊 Experiências Profissionais** - Histórico completo e organizado
- **🏆 Certificações** - Filtradas por categoria (Backend, Frontend, Cloud, etc.)
- **💻 Projetos** - Com tecnologias associadas e responsabilidades
- **🛠 Habilidades** - Nível de proficiência e tempo de experiência
- **📞 Contato** - Todas as formas de entrar em contato

### 🔐 Acesso Restrito (Owner)

- **⚡ CRUD Completo** - Gerenciamento total de todas as entidades
- **🔐 Autenticação JWT** - Com refresh tokens e revogação
- **📝 Logs de Auditoria** - Rastreabilidade de todas as operações
- **🛡️ Validações** - Dados consistentes e seguros

## 🏗️ Tecnologias Utilizadas

| Camada           | Tecnologias                                                     |
| ---------------- | --------------------------------------------------------------- |
| **Backend**      | .NET 8, ASP.NET Core Web API, Entity Framework Core             |
| **Banco**        | SQL Server com migrações code-first                             |
| **Autenticação** | JWT Bearer Tokens, Refresh Tokens, Roles                        |
| **Segurança**    | Hashing de senhas, Validação de modelos, Middleware customizado |
| **Qualidade**    | AutoMapper, Injeção de Dependência, Logging estruturado         |
| **Infra**        | Background Services, Tratamento global de exceções              |

## ☁️ Infraestrutura e Deploy

- **Banco de Dados** hospedado no **Azure SQL Database**  
- **API Backend** publicada no **Azure App Service**  
- **Proximos passos** Pipeline CI/CD via Azure DevOps para automação de builds e deploys  

🔗 **Acesse a API em produção:**  _(em construção)_


## 📡 Endpoints Principais

### Autenticação

```http
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
```

### Consultas Públicas

```http
GET /api/project               # Todos os projetos
GET /api/project/skill/{skill} # Projetos por tecnologia
GET /api/certification         # Todas as certificações
GET /api/experience            # Experiências profissionais
GET /api/skill                 # Habilidades técnicas
GET /api/contact               # Informações de contato
```

### Gestão (Owner)

- Cadastro de novas informações
- Atualização de registros existentes
- Exclusão de dados
- Acesso permitido apenas mediante login/autenticação


## 🏛️ Arquitetura

```
CurriculoInterativo.Api/
├── Controllers/     # Endpoints da API
├── DTOs/            # Objetos de transferência de dados
├── Enums/           # Definições de enums usados no domínio
├── Models/          # Entidades do domínio
├── Mapping/         # Perfis de mapeamento (AutoMapper)
├── Migrations/      # Evolução do schema do banco (EF Core)
├── Repositories/    # Acesso a dados (Pattern Repository)
├── Services/        # Lógica de negócio e orquestração
└── Utils/           # Middleware, Filters, Extensions e Exceptions
```

## 🔒 Segurança Implementada

✅ JWT Tokens - Autenticação stateless  
✅ Refresh Tokens - Rotação segura de credenciais  
✅ Role-based Authorization - Controle de acesso granular  
✅ Password Hashing - Senhas protegidas com bcrypt  
✅ Input Validation - Validação em múltiplas camadas  
✅ Global Exception Handling - Erros tratados consistentemente  
✅ Audit Logging - Rastreabilidade completa

## 📊 Modelo de Dados

_(em construção)_

## 🎯 Próximas Evoluções

_(em construção)_

## 👨‍💻 Autor

**Cimara Sá** - Desenvolvedora apaixonado por criar soluções inovadoras

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/cimarasah)

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/cimarasa/)  

[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:cimarasah@gmail.com)

💡 Curiosidade: Este projeto não é apenas um currículo - é uma demonstração prática de como encaro desenvolvimento de software: com arquitetura limpa, segurança robusta e foco na experiência do usuário final.

⭐ Se este projeto te impressionou, deixe uma star no repositório!
