using Flashcards.Api.App.Endpoints;
using Flashcards.Api.App.Extensions;
using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

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

UseEndpoints(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app.MapGroup("api").RequireCors().WithOpenApi();

    var identityOptions = app.Services.GetRequiredService<IOptions<IdentityOptions>>();

    CardEndpoints.MapEndpoints(endpointsBase);
    UserEndpoints.MapEndpoints(endpointsBase);
    endpointsBase.UseCardCollectionEndpoints(identityOptions);
    LessonEndpoints.MapEndpoints(endpointsBase);
}

public partial class Program { }
