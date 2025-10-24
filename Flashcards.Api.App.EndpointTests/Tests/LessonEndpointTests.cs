using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common.Models.CardCollection;
using Flashcards.Common.Models.Lesson;
using Flashcards.Common.Models.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class LessonEndpointsTests :  IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly TestWebAppFactory _app;
        private readonly Lazy<HttpClient> _client;
        private readonly UserMapper _userMapper = new UserMapper();
        private readonly CardCollectionMapper _collectionMapper = new CardCollectionMapper();
        public LessonEndpointsTests()
        {
            _app = new TestWebAppFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }

        // Storage is registered empty by default
        private IStorage GetStorageInstance() => _app.Services.GetRequiredService<IStorage>();

        [Fact]
        public async Task Get_OnEmptyStorage_Returns_None()
        {
            // Arrange
            var storage = GetStorageInstance();

            // Act
            var response = await _client.Value.GetAsync("/api/lesson/all");

            // Assert
            response.EnsureSuccessStatusCode();

            var lessons = await response.Content.ReadFromJsonAsync<ICollection<LessonListModel>>();
            Assert.NotNull(lessons);
            Assert.Empty(lessons);
        }

        [Fact]
        public async Task Get_OnSeededStorage_Returns_NotEmpty()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            // Act
            var response = await _client.Value.GetAsync("/api/lesson/all");

            // Assert
            response.EnsureSuccessStatusCode();

            var lessons = await response.Content.ReadFromJsonAsync<ICollection<LessonListModel>>();
            Assert.NotNull(lessons);
            Assert.NotEmpty(lessons);
        }

        [Fact]
        public async Task Delete_ExistingEntity_RemovesEntity()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var existingEntity = storage.Lessons[0];

            // Act
            var response = await _client.Value.DeleteAsync($"/api/lesson/{existingEntity.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(storage.Lessons.Any(l => l.Id == existingEntity.Id));
        }

        [Fact]
        public async Task Post_NewEntity_AddsEntity()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var newLesson = new LessonDetailModel
            {
                User = _userMapper.ToDetailModel(storage.Users[0]),
                CardCollection = _collectionMapper.ToDetailModel(storage.Collections[0]),
                Id = new("baaa6f8c-bb86-4066-8696-896c2f14e1ca"),
                Start = DateTime.UtcNow,
                Seen = 3,
                Correct = 2,
                Wrong = 0
            };

            var json = JsonSerializer.Serialize(newLesson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.Value.PostAsync("/api/lesson", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(storage.Lessons.Any(l => l.Id == newLesson.Id));
        }
    }
}
