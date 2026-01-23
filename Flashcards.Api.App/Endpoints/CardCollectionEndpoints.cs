using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class CardCollectionEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var cardCollectionEndpoints = endpointRouteBuilder.MapGroup("cardCollection")
            .WithTags("cardCollection");
        cardCollectionEndpoints.RequireAuthorization();

        cardCollectionEndpoints.MapGet("", (ICardCollectionFacade cardCollectionFacade) => cardCollectionFacade.GetAll());

        cardCollectionEndpoints.MapPost("query",
            (CardCollectionQueryModel query, ICardCollectionFacade cardCollectionFacade) =>
                cardCollectionFacade.GetByQuery(query));

        cardCollectionEndpoints.MapGet("{id:guid}", Results<Ok<CardCollectionDetailModel>, NotFound<string>> (Guid id, ICardCollectionFacade cardCollectionFacade)
            => cardCollectionFacade.GetById(id) is { } cardCollection
                ? TypedResults.Ok(cardCollection)
                : TypedResults.NotFound("This card collection was not found!"));

        cardCollectionEndpoints.MapGet("user/{ownerId}", (string ownerId, ICardCollectionFacade cardCollectionFacade) =>
        {
            var result = cardCollectionFacade.GetByOwner(ownerId);
            return TypedResults.Ok(result);
        });

        var cardCollectionModifyingEndpoints = cardCollectionEndpoints.MapGroup("");

        if (identityOptions.Value.IsIdentityEnabled)
        {
            cardCollectionModifyingEndpoints.RequireAuthorization();
        }

        cardCollectionModifyingEndpoints.MapPost("", (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            return cardCollectionFacade.Create(cardCollection, userRoles, userId);
        });

        cardCollectionModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                return TypedResults.Ok(cardCollectionFacade.Update(cardCollection, userRoles, userId));
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        cardCollectionEndpoints.MapPost("upsert", (CardCollectionDetailModel cardCollection, ICardCollectionFacade cardCollectionFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            var id = cardCollectionFacade.CreateOrUpdate(cardCollection, userRoles, userId);
            return TypedResults.Ok(id);
        });

        var cardCollectionDeleteEndpoint = cardCollectionModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> (Guid id, ICardCollectionFacade cardCollectionFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                cardCollectionFacade.Delete(id, userRoles, userId);
                return TypedResults.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        if (identityOptions.Value.IsIdentityEnabled)
        {
            // // Uncomment this to only enable admins to delete card collections
            // cardCollectionDeleteEndpoint.RequireAuthorization(ApiPolicies.CardCollectionAdmin);
        }

        return endpointRouteBuilder;
    }
}
