using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Models.Card;
using Flashcards.Common.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<Storage>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardFacade, CardFacade>();

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserFacade, UserFacade>();


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
    UseUserEndpoints(endpointsBase);
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