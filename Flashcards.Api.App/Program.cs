using Flashcards.Api.App.Endpoints;
using Flashcards.Api.App.Extensions;
using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<IStorage, Storage>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardFacade, CardFacade>();

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserFacade, UserFacade>();

builder.Services.AddSingleton<CardCollectionMapper>();
builder.Services.AddTransient<ICardCollectionRepository, CardCollectionRepository>();
builder.Services.AddScoped<ICardCollectionFacade, CardCollectionFacade>();

builder.Services.AddSingleton<LessonMapper>();
builder.Services.AddScoped<ILessonFacade, LessonFacade>();
builder.Services.AddTransient<ILessonRepository, LessonRepository>();

builder.Services.AddHttpContextAccessor();

// Configure Identity Server client for M2M communication from appsettings.json
builder.Services.Configure<IdentityServerOptions>(
    builder.Configuration.GetSection("IdentityServer"));

builder.Services.AddHttpClient<IdentityServerClient>();

// Get Identity Server authority from configuration for JWT validation
var identityServerAuthority = builder.Configuration["IdentityServer:Authority"] 
    ?? throw new InvalidOperationException("IdentityServer:Authority is not configured in appsettings.json");

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Identity Server authority from configuration
        options.Authority = identityServerAuthority;
        
        // Validate the audience (which API this token is for)
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false, // For now, we'll validate scope instead
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        
        // For development: accept localhost certificates
        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false; // Only for development!
        }
    });

builder.Services.AddAuthorization(options =>
{
    // Require the "flashcardsapi" scope for all API calls
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "flashcardsapi");
    });
    
    // Optional: Admin-only policy
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "flashcardsapi");
        policy.RequireRole("Admin");
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCors();
app.UseHttpsRedirection();

// IMPORTANT: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

UseEndpoints(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app.MapGroup("api")
        .RequireCors()
        .RequireAuthorization("ApiScope") // Protect all API endpoints
        .WithOpenApi();

    var identityOptions = app.Services.GetRequiredService<IOptions<IdentityOptions>>();

    CardEndpoints.MapEndpoints(endpointsBase);
    endpointsBase.UseUserEndpoints(identityOptions);
    endpointsBase.UseCardCollectionEndpoints(identityOptions);
    LessonEndpoints.MapEndpoints(endpointsBase);
}

public partial class Program { }
