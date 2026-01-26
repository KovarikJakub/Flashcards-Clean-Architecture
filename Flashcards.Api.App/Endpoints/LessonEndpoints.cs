using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Flashcards.Common.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Flashcards.Api.App.Endpoints;

public class LessonEndpoints(IOptions<IdentityOptions> identityOptions) : EndpointsBase
{
    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lessonEndpoints = endpointRouteBuilder
            .MapGroup("lesson")
            .WithTags("lesson");

        lessonEndpoints.MapGet("", 
            (ILessonFacade lessonFacade) =>
            {
                return TypedResults.Ok(lessonFacade.GetAll());
            });

        lessonEndpoints.MapPost("query", 
            (LessonQueryModel query, ILessonFacade lessonFacade) =>
            {
                return TypedResults.Ok(lessonFacade.GetByQuery(query));
            });

        lessonEndpoints.MapGet("{id:guid}", Results<Ok<LessonDetailModel>, NotFound<string>> 
            (Guid id, ILessonFacade lessonFacade) =>
            {
                return lessonFacade.GetById(id) is { } lesson
                    ? TypedResults.Ok(lesson)
                    : TypedResults.NotFound("This lesson was not found!");
            });

        var lessonModifyingEndpoints = lessonEndpoints.MapGroup("");
        if (identityOptions.Value.IsIdentityEnabled)
        {
            lessonModifyingEndpoints.RequireAuthorization();
        }

        lessonModifyingEndpoints.MapPost("", 
            (LessonCreateModel lesson, ILessonFacade lessonFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                var id = lessonFacade.Create(lesson, userRoles, userId!);
                return TypedResults.Ok(id);
            });

        lessonModifyingEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> 
            (Guid id, ILessonFacade lessonFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);
                ThrowIfUserIdIsNull(userId);

                try
                {
                    lessonFacade.Delete(id, userRoles, userId!);
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
