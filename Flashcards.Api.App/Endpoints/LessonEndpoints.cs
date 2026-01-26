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

        lessonEndpoints.MapGet("", (ILessonFacade lessonFacade) =>
        {
            var result = lessonFacade.GetAll();
            return TypedResults.Ok(result);
        });

        lessonEndpoints.MapPost("query", (LessonQueryModel query, ILessonFacade lessonFacade) =>
        {
            var result = lessonFacade.GetByQuery(query);
            return TypedResults.Ok(result);
        });

        lessonEndpoints.MapGet("{id:guid}", Results<Ok<LessonDetailModel>, NotFound<string>> (Guid id, ILessonFacade lessonFacade)
            => lessonFacade.GetById(id) is { } lesson
                ? TypedResults.Ok(lesson)
                : TypedResults.NotFound("This lesson was not found!"));

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
                
                return lessonFacade.Create(lesson, userRoles, userId);
            });

        lessonModifyingEndpoints.MapDelete("{id:guid}", 
            (Guid id, ILessonFacade lessonFacade, IHttpContextAccessor httpContextAccessor) =>
            {
                var userId = GetUserId(httpContextAccessor);
                var userRoles = GetUserRoles(httpContextAccessor);

                lessonFacade.Delete(id, userRoles, userId);
            });

        return endpointRouteBuilder;
    }
}
