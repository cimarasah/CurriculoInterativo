using AutoMapper;
using CurriculoInterativo.Api;
using CurriculoInterativo.Api.Mapping;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.CertificationRepository;
using CurriculoInterativo.Api.Repositories.ContactRepository;
using CurriculoInterativo.Api.Repositories.ExperienceRepository;
using CurriculoInterativo.Api.Repositories.ProjectRepository;
using CurriculoInterativo.Api.Repositories.RefreshTokenRepository;
using CurriculoInterativo.Api.Repositories.SkillRepository;
using CurriculoInterativo.Api.Repositories.UserRepository;
using CurriculoInterativo.Api.Services.AuthService;
using CurriculoInterativo.Api.Services.CertificationService;
using CurriculoInterativo.Api.Services.ContactService;
using CurriculoInterativo.Api.Services.ExperienceService;
using CurriculoInterativo.Api.Services.ProjectService;
using CurriculoInterativo.Api.Services.SkillService;
using CurriculoInterativo.Api.Services.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ResumeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ResumeDb"),
        sqlOptions =>
        {
            // Retry para transient faults
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // Aumenta timeout de comandos (se necessário)
            sqlOptions.CommandTimeout(60); // segundos
        }
    )
);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// AutoMapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Configuração do Identity para hash de senhas
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();



// Repositories
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


// Services
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Apenas para desenvolvimento
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Falha na autenticação: {Exception}", context.Exception);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Token inválido ou ausente para: {Path}", context.Request.Path);
            return Task.CompletedTask;
        }
    };
});

// Configuração de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001") // URLs do seu frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configuração dos controllers
builder.Services.AddControllers();

// Configuração do Swagger com suporte a JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currículo Interativo API",
        Version = "v1",
        Description = "API para gerenciamento de currículo interativo com autenticação JWT"
    });

    // Configuração do JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

//Configuraçãos para o frontend
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
