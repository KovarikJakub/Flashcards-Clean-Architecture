using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class UserEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var userEndpoints = endpointRouteBuilder.MapGroup("user")
            .WithTags("user");

       
        userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll());

        userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
            => userFacade.GetById(id) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound("This user was not found!"));

       
        var userModifyingEndpoints = userEndpoints.MapGroup("");

        if (identityOptions.Value.IsIdentityEnabled)
        {
            userModifyingEndpoints.RequireAuthorization();
        }

        userModifyingEndpoints.MapPost("", (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            return userFacade.Create(user, userRoles, userId);
        });

        userModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                return TypedResults.Ok(userFacade.Update(user, userRoles, userId));
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        userModifyingEndpoints.MapPost("upsert", (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            var id = userFacade.CreateOrUpdate(user, userRoles, userId);
            return TypedResults.Ok(id);
        });

        userModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> (Guid id, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                userFacade.Delete(id, userRoles, userId);
                return TypedResults.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        return endpointRouteBuilder;
    }
}