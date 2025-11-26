using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class LessonEndpointsTests : BaseEndpointTests<LessonEntity, LessonDetailModel, LessonListModel>, IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly UserMapper UserMapper = new();
        private static readonly CardCollectionMapper CollectionMapper = new();
        private static readonly LessonMapper LessonMapper = new(UserMapper, CollectionMapper);

        protected override string EndpointBase => "/api/lesson";
        protected override string EndpointGetAll => "/api/lesson/all";
        protected override Guid GetEntityId(LessonEntity entity) => entity.Id;
        protected override Guid GetModelId(LessonDetailModel model) => model.Id;
        protected override IList<LessonEntity> GetCollection() => _storage.Lessons;
        protected override LessonDetailModel CreateNewModel() => new() // new model for testing insert
        {
            User = UserMapper.ToDetailModel(_storage.Users.First()),
            CardCollection = CollectionMapper.ToDetailModel(_storage.Collections.First()),
            Id = new("baaa6f8c-bb86-4066-8696-896c2f14e1ca"),
            Start = DateTime.UtcNow,
            Seen = 3,
            Correct = 2,
            Wrong = 0
        };
        protected override LessonDetailModel GetUpdatedModel(LessonEntity entity)
            => LessonMapper.ToDetailModel(entity); // the update test is not run for Lesson entity
        protected override bool VerifyUpdated(LessonDetailModel updatedModel) => true;

        [Fact]
        public override Task Put_OnExistingEntity_UpdatesExistingEntity()
        {
            // Ignore this test since Lesson does not have an Update endpoint
            Assert.True(true);
            return Task.CompletedTask;
        }
    }
}
