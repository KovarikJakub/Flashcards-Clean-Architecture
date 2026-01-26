using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class CardEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var cardEndpoints = endpointRouteBuilder
            .MapGroup("card")
            .WithTags("card");

        cardEndpoints.MapGet("", (ICardFacade cardFacade) => cardFacade.GetAll());

        cardEndpoints.MapPost("query", (CardQueryModel query, ICardFacade cardFacade) => cardFacade.GetByQuery(query));

        cardEndpoints.MapGet("{id:guid}", Results<Ok<CardDetailModel>, NotFound<string>> (Guid id, ICardFacade cardFacade)
            => cardFacade.GetById(id) is { } card
                ? TypedResults.Ok(card)
                : TypedResults.NotFound("This card was not found!"));

        var cardModifyingEndpoints = cardEndpoints.MapGroup("");
        if (identityOptions.Value.IsIdentityEnabled)
        {
            cardModifyingEndpoints.RequireAuthorization();
        }

        cardModifyingEndpoints.MapPost("", 
            (CardCreateModel card, ICardFacade cardFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                return cardFacade.Create(card, userRoles, userId);
            });

        cardModifyingEndpoints.MapPut("", 
            (CardDetailModel card, ICardFacade cardFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                return cardFacade.Update(card, userRoles, userId);
            });

        cardModifyingEndpoints.MapPost("upsert",
            (CardDetailModel card, ICardFacade cardFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                return cardFacade.CreateOrUpdate(card, userRoles, userId);
            });

        cardModifyingEndpoints.MapDelete("{id:guid}",
            (Guid id, ICardFacade cardFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                cardFacade.Delete(id, userRoles, userId);
            });
         
        return endpointRouteBuilder;
    }
}
