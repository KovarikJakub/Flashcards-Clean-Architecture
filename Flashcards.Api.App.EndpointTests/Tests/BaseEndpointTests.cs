using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public abstract class BaseEndpointTests<TEntity, TDetailModel, TListModel>
        where TEntity : class, IWithGuid
        where TDetailModel : class, IWithGuid
        where TListModel : class, IWithGuid
    {
        protected readonly Lazy<HttpClient> _client;
        protected readonly TestWebAppFactory _app;
        protected readonly IStorage _storage;

        protected BaseEndpointTests()
        {
            _app = new TestWebAppFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
            _storage = _app.Services.GetRequiredService<IStorage>();
        }

        protected abstract string EndpointBase { get; }
        protected abstract string EndpointGetAll { get; }
        protected abstract IList<TEntity> GetCollection();
        protected abstract Guid GetEntityId(TEntity entity);
        protected abstract Guid GetModelId(TDetailModel model);
        protected abstract TDetailModel CreateNewModel(); // Should return new DM for testing Insert of new entry
        protected abstract TDetailModel GetUpdatedModel(TEntity entity);
        protected abstract bool VerifyUpdated(TDetailModel updatedModel);

        [Fact]
        public async Task Delete_ExistingEntity_RemovesEntity()
        {
            // Arrange
            _storage.Seed();
            var existingEntity = GetCollection().First();

            // Act
            var response = await _client.Value.DeleteAsync($"{EndpointBase}/{GetEntityId(existingEntity)}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain(GetCollection(), e => GetEntityId(e).Equals(GetEntityId(existingEntity)));
        }

        [Fact]
        public async Task Get_OnEmptyStorage_DoesNotFailAndReturnsEmpty()
        {
            // Arrange

            // Act
            var response = await _client.Value.GetAsync(EndpointGetAll);

            // Assert
            response.EnsureSuccessStatusCode();

            var entities = await response.Content.ReadFromJsonAsync<ICollection<TListModel>>();
            Assert.NotNull(entities);
            Assert.Empty(entities);
        }

        [Fact]
        public async Task Get_OnSeededStorage_ReturnsCorrectly()
        {
            // Arrange
            _storage.Seed();

            // Act
            var response = await _client.Value.GetAsync(EndpointGetAll);

            // Assert
            response.EnsureSuccessStatusCode();

            var lessons = await response.Content.ReadFromJsonAsync<ICollection<TListModel>>();
            Assert.NotNull(lessons);
            Assert.NotEmpty(lessons);
        }

        [Fact]
        public async Task Post_NewEntity_AddsEntity()
        {
            // Arrange
            _storage.Seed();
            var newModel = CreateNewModel();

            var json = JsonSerializer.Serialize(newModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.Value.PostAsync(EndpointBase, content);

            // Assert
            response.EnsureSuccessStatusCode();
            var newId = GetModelId(newModel);
            Assert.Contains(GetCollection(), e => GetEntityId(e) == newId);
        }

        [Fact]
        public virtual async Task Put_OnExistingEntity_UpdatesExistingEntity()
        {
            // Arrange
            _storage.Seed();

            // Get some existing entity
            var existingEntity = GetCollection().First();
            // Make some changes to it
            var updatedModel = GetUpdatedModel(existingEntity);

            // Act
            var json = JsonSerializer.Serialize(updatedModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.Value.PutAsync(EndpointBase, content);

            // Assert
            response.EnsureSuccessStatusCode();
            // Verify that there are changes made on the model
            Assert.True(VerifyUpdated(updatedModel));
        }
    }
}
