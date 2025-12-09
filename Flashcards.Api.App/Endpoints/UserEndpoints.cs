using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Flashcards.Api.App.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapEndpoints(this RouteGroupBuilder group)
    {
        var userEndpoints = group.MapGroup("user").WithTags("user");

        userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll());

        userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
            => userFacade.GetById(id) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound("This user was not found!"));

        userEndpoints.MapPost("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Create(user));
        userEndpoints.MapPut("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Update(user));
        userEndpoints.MapPost("upsert", (UserDetailModel user, IUserFacade userFacade) => userFacade.CreateOrUpdate(user));
        userEndpoints.MapDelete("{id:guid}", (Guid id, IUserFacade userFacade) => userFacade.Delete(id));

        return group;
    }
}
