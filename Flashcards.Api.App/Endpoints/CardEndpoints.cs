using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Flashcards.Api.App.Endpoints;

public static class CardEndpoints
{
    public static RouteGroupBuilder MapEndpoints(RouteGroupBuilder group)
    {
        var cardEndpoints = group.MapGroup("card").WithTags("card");
        cardEndpoints.RequireAuthorization();
        cardEndpoints.MapGet("", (ICardFacade cardFacade) => cardFacade.GetAll());

        cardEndpoints.MapGet("{id:guid}", Results<Ok<CardDetailModel>, NotFound<string>> (Guid id, ICardFacade cardFacade)
            => cardFacade.GetById(id) is { } card
                ? TypedResults.Ok(card)
                : TypedResults.NotFound("This card was not found!"));

        cardEndpoints.MapPost("", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.Create(card));
        cardEndpoints.MapPut("", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.Update(card));
        cardEndpoints.MapPost("upsert", (CardDetailModel card, ICardFacade cardFacade) => cardFacade.CreateOrUpdate(card));
        cardEndpoints.MapDelete("{id:guid}", (Guid id, ICardFacade cardFacade) => cardFacade.Delete(id));

        return group;
    }
}
