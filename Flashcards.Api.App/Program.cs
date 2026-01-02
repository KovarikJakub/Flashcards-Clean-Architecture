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
using Flashcards.Common;

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

builder.Services.AddHttpClient<IdentityServerClient>();

ConfigureOptions(builder.Services);
ConfigureCors(builder.Services);
ConfigureAuthentication(builder.Services, builder.Configuration);

var app = builder.Build();

UseSecurityFeatures(app);
UseAuthenticationAndAuthorization(app);
UseEndpoints(app);
UseOpenApi(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app.MapGroup("api")
        .WithOpenApi();

    var identityOptions = app.Services.GetRequiredService<IOptions<IdentityOptions>>();

    CardEndpoints.MapEndpoints(endpointsBase);
    endpointsBase.UseUserEndpoints(identityOptions);
    endpointsBase.UseCardCollectionEndpoints(identityOptions);
    LessonEndpoints.MapEndpoints(endpointsBase);
}

void ConfigureAuthentication(IServiceCollection serviceCollection, IConfiguration configuration)
{
    var identityOptions = configuration.GetSection("IdentityOptions").Get<IdentityOptions>();

    serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = identityOptions?.Authority;
            options.TokenValidationParameters.ValidateAudience = false;
        });

    serviceCollection.AddAuthorizationBuilder()
        .AddPolicy("admin", policy =>
            policy.RequireAuthenticatedUser()
                .RequireRole(UserRoles.Admin)
        );

    serviceCollection.AddHttpContextAccessor();
}

void ConfigureOptions(IServiceCollection serviceCollection)
{
    serviceCollection.Configure<IdentityOptions>(builder.Configuration.GetSection(nameof(IdentityOptions)));
    serviceCollection.Configure<IdentityServerOptions>(builder.Configuration.GetSection("IdentityServer"));
}

void ConfigureCors(IServiceCollection serviceCollection)
{
    serviceCollection.AddCors(options =>
    {
        options.AddDefaultPolicy(o =>
            o.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

void UseSecurityFeatures(IApplicationBuilder application)
{
    application.UseCors();
    application.UseHttpsRedirection();
}

void UseAuthenticationAndAuthorization(IApplicationBuilder application)
{
    application.UseAuthentication();
    application.UseAuthorization();
}
void UseOpenApi(IApplicationBuilder application)
{
    application.UseOpenApi();
    application.UseSwaggerUi();
}

public partial class Program { }
