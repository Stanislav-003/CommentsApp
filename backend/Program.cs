using backend.Abstractions.Messaging;
using backend.Behaviors;
using backend.Database;
using backend.Decorators;
using backend.Extensions;
using backend.Middlewares;
using backend.Models;
using backend.Options;
using backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEndpoints();

#region Serilog

var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext();

#if DEBUG
loggerConfig.MinimumLevel.Debug();
#else
        loggerConfig.MinimumLevel.Warning();
#endif

loggerConfig.WriteTo.Console();

Log.Logger = loggerConfig.CreateLogger();
builder.Services.AddSerilog();
builder.Host.UseSerilog();

#endregion

#region HealthChecks

builder.Services.AddHealthChecks()
    .AddCheck("Liveness", () => HealthCheckResult.Healthy("Liveness check passed"))
    .AddCheck("Readiness", () =>
    {
        bool isDatabaseReady = CheckDatabaseConnection();
        return isDatabaseReady
            ? HealthCheckResult.Healthy("Readiness check passed")
            : HealthCheckResult.Unhealthy("Database is not ready");
    });

#endregion

#region ApplicationSrervices

string connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

// Options
builder.Services.Configure<JwtSettings>(builder.Configuration);
builder.Services.Configure<BlobStorage>(builder.Configuration.GetSection("AzureBlobStorage"));

builder.Services.AddSingleton<BlobStorageService>();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#endregion

#region Scrutor

builder.Services.Scan(scan => scan.FromAssembliesOf(typeof(Program))
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
        .AsImplementedInterfaces().WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
        .AsImplementedInterfaces().WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
        .AsImplementedInterfaces().WithScopedLifetime());

builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
//builder.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));
//builder.Services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

#endregion

#region Auth

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtIssuer"] ?? throw new InvalidOperationException("JwtIssuer is not configured."),
        ValidAudience = builder.Configuration["JwtAudience"] ?? throw new InvalidOperationException("JwtAudience is not configured."),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecurityKey"] ?? throw new InvalidOperationException("SecurityKey is not configured.")))
    };
});

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
builder.Services.AddAuthorization();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TASCO-PANEL API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);

    opt.UseInlineDefinitionsForEnums();

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

#endregion

#region CORS

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

#endregion

bool CheckDatabaseConnection()
{
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ConnectionStrings:DataAccessPostgreSqlProvider is not configured.");

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return true;
    }
    catch (Exception ex)
    {
        Log.Error("Database connection failed: {Error}", ex.Message);
        return false;
    }
}

var app = builder.Build();
Log.Warning("Starting application...");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    Log.Information("Applying database migrations...");
    app.ApplyMigrations();
    Log.Information("Database migrations completed successfully");

    //app.SeedData();
}

if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapEndpoints();

app.MapGet("/", () => "Backend is running!")
    .RequireAuthorization(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireRole(UsersRoles.USER)
    .Build())
    .WithTags("Helper");

app.Run();