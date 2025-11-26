using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // In Development, show the Api docs under /swagger route:
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCors();
app.UseHttpsRedirection();

UseEndpoints(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app
        .MapGroup("api")
        .RequireCors()
        .WithOpenApi();

    UseCardsEndpoints(endpointsBase);
    UseUserEndpoints(endpointsBase);
    UseCardCollectionEndpoints(endpointsBase);
    UseLessonEndpoints(endpointsBase);
}

void UseCardsEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var cardEndpoints = routeGroupBuilder.MapGroup("card").WithTags("card");

    cardEndpoints.MapGet("", (ICardFacade cardFacade) => cardFacade.GetAll());

    cardEndpoints.MapGet("{id:guid}", Results<Ok<CardDetailModel>, NotFound<string>> (Guid id, ICardFacade cardFacade)
        => cardFacade.GetById(id) is { } card
            ? TypedResults.Ok(card)
            : TypedResults.NotFound("This card was not found!"));

    cardEndpoints.MapPost("", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.Create(card));
    cardEndpoints.MapPut("", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.Update(card));
    cardEndpoints.MapPost("upsert", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.CreateOrUpdate(card));
    cardEndpoints.MapDelete("{id:guid}", (Guid id, ICardFacade cardFacade) => cardFacade.Delete(id));
}

void UseUserEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var userEndpoints = routeGroupBuilder.MapGroup(("user")).WithTags("user");

    userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll());

    userEndpoints.MapGet("{id:guid}",Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
            => userFacade.GetById(id) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound("This user was not found!"));

    userEndpoints.MapPost("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Create(user));
    userEndpoints.MapPut("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Update(user));
    userEndpoints.MapPost("upsert", (UserDetailModel user, IUserFacade userFacade) => userFacade.CreateOrUpdate(user));
    userEndpoints.MapDelete("{id:guid}", (Guid id, IUserFacade userFacade) => userFacade.Delete(id));
}

void UseCardCollectionEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var cardCollectionEndpoints = routeGroupBuilder.MapGroup("cardCollection").WithTags("cardCollection");

    cardCollectionEndpoints.MapGet("", (ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.GetAll());

    cardCollectionEndpoints.MapGet("{id:guid}", Results<Ok<CardCollectionDetailModel>, NotFound<string>> (Guid id, ICardCollectionFacade cardCollectionFacade)
        => cardCollectionFacade.GetById(id) is { } cardCollection
            ? TypedResults.Ok(cardCollection)
            : TypedResults.NotFound("This card collection was not found!"));

    cardCollectionEndpoints.MapPost("", (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.Create(cardCollection));
    cardCollectionEndpoints.MapPut("", (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.Update(cardCollection));
    cardCollectionEndpoints.MapPost("upsert", (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.CreateOrUpdate(cardCollection));
    cardCollectionEndpoints.MapDelete("{id:guid}", (Guid id, ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.Delete(id));
}

void UseLessonEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var lessonEndpoints = routeGroupBuilder.MapGroup("lesson").WithTags("lesson");

    lessonEndpoints.MapGet("all", (ILessonFacade lessonFacade) => lessonFacade.GetAll());
    
    lessonEndpoints.MapGet("", (
        ILessonFacade lessonFacade,
        [AsParameters] LessonQueryModel query) =>
    {
        var result = lessonFacade.GetByQuery(query);
        return TypedResults.Ok(result);
    });

    lessonEndpoints.MapGet("{id:guid}", Results<Ok<LessonDetailModel>, NotFound<string>> (Guid id, ILessonFacade lessonFacade)
        => lessonFacade.GetById(id) is { } lesson
            ? TypedResults.Ok(lesson)
            : TypedResults.NotFound("This lesson was not found!"));

    lessonEndpoints.MapPost("", (LessonDetailModel lesson, ILessonFacade lessonFacade) => lessonFacade.Create(lesson));
    lessonEndpoints.MapDelete("{id:guid}", (Guid id, ILessonFacade lessonFacade) => lessonFacade.Delete(id));
}

public partial class Program { }