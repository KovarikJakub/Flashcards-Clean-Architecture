using Flashcards.Api.BL.Facades;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Flashcards.Api.App.Endpoints;

public static class LessonEndpoints
{
    public static RouteGroupBuilder MapEndpoints(this RouteGroupBuilder group)
    {
        var lessonEndpoints = group.MapGroup("lesson").WithTags("lesson");

        lessonEndpoints.MapGet("all", (ILessonFacade lessonFacade) => lessonFacade.GetAll());

        lessonEndpoints.MapGet("", (
            ILessonFacade lessonFacade,
            [AsParameters] LessonQueryModel query) =>
        {
            var result = lessonFacade.GetByQuery(query);
            return TypedResults.Ok(result);
        });

        lessonEndpoints.MapGet("{id:guid}", Results<Ok<LessonDetailModel>, NotFound<string>> (Guid id, ILessonFacade lessonFacade)
            => lessonFacade.GetById(id) is { } lesson
                ? TypedResults.Ok(lesson)
                : TypedResults.NotFound("This lesson was not found!"));

        lessonEndpoints.MapPost("", (LessonDetailModel lesson, ILessonFacade lessonFacade) => lessonFacade.Create(lesson));
        lessonEndpoints.MapDelete("{id:guid}", (Guid id, ILessonFacade lessonFacade) => lessonFacade.Delete(id));

        return group;
    }
}
