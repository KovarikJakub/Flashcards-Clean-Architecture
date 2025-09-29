using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Models.Card;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<Storage>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardFacade, CardFacade>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // In Development, show the Api docs under /swagger route:
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

UseEndpoints(app);

app.Run();

void UseEndpoints(WebApplication app)
{
    var endpointsBase = app.MapGroup("api").WithOpenApi();

    UseCardsEndpoints(endpointsBase);
    // TODO: use other endpoints here
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