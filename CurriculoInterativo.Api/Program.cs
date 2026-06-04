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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using CurriculoInterativo.Api.Services.CurriculumService;
using CurriculoInterativo.Api.Services.PdfService;
using CurriculoInterativo.Api.Repositories.SuggestionRepository;
using CurriculoInterativo.Api.Services.AIService;
using CurriculoInterativo.Api.Services.DedicatedCurriculumService;
using CurriculoInterativo.Api.Repositories.PasswordResetTokenRepository;
using CurriculoInterativo.Api.Services.EmailService;
using CurriculoInterativo.Api.Services.GoogleAuthService;
using CurriculoInterativo.Api.Repositories.CurriculumGenerationRepository;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

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

            // Aumenta timeout de comandos (se necess�rio)
            sqlOptions.CommandTimeout(60); // segundos
        }
    )
);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// HttpContextAccessor para capturar IP e User-Agent
builder.Services.AddHttpContextAccessor();


// AutoMapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Configura��o do Identity para hash de senhas
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();



// Repositories
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ISuggestionRepository, SuggestionRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<ICurriculumGenerationRepository, CurriculumGenerationRepository>();



builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIService, GoogleGeminiService>();
builder.Services.AddScoped<IDedicatedCurriculumService, DedicatedCurriculumService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

var dataProtectionKeysPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CurriculoInterativo", "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("CurriculoInterativo");

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey n�o configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie("Cookies", cookieOptions =>
{
    cookieOptions.Cookie.Name = "CurriculoInterativo.Auth";
    cookieOptions.Cookie.HttpOnly = true;
    cookieOptions.Cookie.SameSite = SameSiteMode.Lax;
    cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    cookieOptions.SlidingExpiration = true;
    cookieOptions.Cookie.Path = "/";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleSettings = builder.Configuration.GetSection("GoogleOAuth");
    options.ClientId = googleSettings["ClientId"] ?? throw new InvalidOperationException("Google ClientId não configurado");
    options.ClientSecret = googleSettings["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret não configurado");
    options.CallbackPath = "/api/auth/google-callback";
    options.SaveTokens = false;
    options.SignInScheme = "Cookies";
    
    options.Events.OnRemoteFailure = async context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError("Erro no OAuth do Google: {Error}", context.Failure?.Message);
        logger.LogError("Stack trace: {StackTrace}", context.Failure?.StackTrace);
        context.HandleResponse();
        context.Response.Redirect($"/index.html?error=google_auth_failed&details={Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error")}");
        await Task.CompletedTask;
    };
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
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
            logger.LogError("Falha na autentica��o: {Exception}", context.Exception);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Token inv�lido ou ausente para: {Path}", context.Request.Path);
            return Task.CompletedTask;
        }
    };
});

// Configura��o de autoriza��o
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
});

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configura��o dos controllers
builder.Services.AddControllers();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


// Configura��o do Swagger com suporte a JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Curr�culo Interativo API",
        Version = "v1",
        Description = "API para gerenciamento de curr�culo interativo com autentica��o JWT"
    });

    // Configura��o do JWT no Swagger
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

    // Incluir coment�rios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configura��o de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseForwardedHeaders();
//Configura��os para o frontend
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANTE: UseAuthentication deve vir ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
