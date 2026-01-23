using Flashcards.Api.BL.Facades;
using Flashcards.Common.Enums;
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
        userEndpoints.RequireAuthorization();

        userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll());

        userEndpoints.MapPost("query",
            (UserQueryModel query, IUserFacade userFacade) =>
                userFacade.GetByQuery(query));

        userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
            => userFacade.GetById(id) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound("This user was not found!"));

        userEndpoints.MapGet("owner/{ownerId}", Results<Ok<UserDetailModel>, NotFound<string>> (string ownerId, IUserFacade userFacade)
            => userFacade.GetByOwnerId(ownerId) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound("User not found for given owner id"));

       
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

        // NEW: Create user with Identity Server integration
        userModifyingEndpoints.MapPost("create-with-identity", async Task<Results<Ok<Guid>, BadRequest<string>>> (
            CreateUserWithIdentityRequest request, 
            IUserFacade userFacade, 
            IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                // Logic removed temporarily
                throw new NotImplementedException("Identity Server integration is temporarily disabled.");
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
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
        
        userModifyingEndpoints.MapDelete("{id:guid}/with-identity", async Task<Results<Ok, ForbidHttpResult, BadRequest<string>>> (
            Guid id, 
            IUserFacade userFacade, 
            IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);

            try
            {
                // Logic removed temporarily
                throw new NotImplementedException("Identity Server integration is temporarily disabled.");
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        });

        return endpointRouteBuilder;
    }
}
